export function initialize(id) {
    const element = document.getElementById(id);
    if (!element) {
        console.warn(`KeyboardShortcutEditor: Element with id '${id}' not found`);
        return null;
    }

    let capturedKeys = {
        modifiers: [],
        keyCode: null
    };

    const instance = {
        element: element,
        isCapturing: false,
    }

    instance.stopCapture = function() {
        if (!this.isCapturing) return;

        this.isCapturing = false;
        this.element.classList.remove('tb-keyboard-capturing');
        this.element.removeAttribute('tabindex');

        // Remove event listeners
        this.element.removeEventListener('keydown', this.handleKeyDown);
        this.element.removeEventListener('keyup', this.handleKeyUp);
        this.element.removeEventListener('blur', this.stopCapture);

        return {
            modifiers: capturedKeys.modifiers,
            keyCode: capturedKeys.keyCode
        };
    }.bind(instance);
       

    instance.startCapture = function () {
        if (this.isCapturing) return;

        this.isCapturing = true;
        this.element.classList.add('tb-keyboard-capturing');
        this.element.setAttribute('tabindex', '0');
        this.element.focus();

        // Reset captured keys
        capturedKeys.modifiers = [];
        capturedKeys.keyCode = null;

        // Add event listeners
        this.element.addEventListener('keydown', this.handleKeyDown);
        this.element.addEventListener('keyup', this.handleKeyUp);
        this.element.addEventListener('blur', this.stopCapture);
    }.bind(instance);

    instance.dispose = function () { 
        this.stopCapture();
    }.bind(instance);

    instance.handleKeyDown = function (event) {

        event.preventDefault();
        event.stopPropagation();
        // Capture modifiers
        const modifiers = [];
        if (event.ctrlKey || event.metaKey) modifiers.push('Ctrl');
        if (event.shiftKey) modifiers.push('Shift');
        if (event.altKey) modifiers.push('Alt');
        // Capture key code (ignore modifier keys themselves)
        if (!['Control', 'Shift', 'Alt', 'Meta'].includes(event.key)) {
            capturedKeys.keyCode = event.code;
        }
        capturedKeys.modifiers = modifiers;

        // Update visual feedback
        instance.dotNetEventListener.invokeMethodAsync("OnShortcutChanging", {
            modifiers: capturedKeys.modifiers,
            keyCode: capturedKeys.keyCode
        });
        
    }.bind(instance);

    instance.handleKeyUp = function (event) {

        // Complete capture on key up of non-modifier keys
        if (!['Control', 'Shift', 'Alt', 'Meta'].includes(event.key) && capturedKeys.keyCode) {
            const result = instance.stopCapture();
            // Trigger custom event with captured shortcut
            const customEvent = new CustomEvent('shortcutCaptured', {
                detail: result,
                bubbles: true
            });

            console.log("shortcutCaptured", customEvent);
            instance.element.dispatchEvent(customEvent);
        }
    }.bind(instance);

    instance.onShortcutCaptured = function (dotNetEventListener) {
        instance.dotNetEventListener = dotNetEventListener;
        instance.element.addEventListener("shortcutCaptured", (event) => {
            instance.dotNetEventListener.invokeMethodAsync("OnShortcutCaptured", event.detail);
        });
    }.bind(instance);
    
    return instance;
}
export function dispose(instance) {
    if (instance && instance.dispose) {
        instance.dispose();
    }
}
