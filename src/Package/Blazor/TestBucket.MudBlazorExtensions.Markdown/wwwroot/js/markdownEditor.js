const _instances = [];

function initialize(dotNetObjectRef, element, elementId, previewElementId, options) {
    const instances = _instances;

    if (!options.toolbar) {
        // remove empty toolbar so that we can fallback to the default items
        delete options.toolbar;
    }
    else if (options.toolbar && options.toolbar.length > 0) {
        // map any named action with a real action from EasyMDE
        options.toolbar.forEach(button => {
            // make sure we don't operate on separators
            if (button !== "|") {
                if (button.action) {
                    if (!button.action.startsWith("http")) {
                        button.action = EasyMDE[button.action];
                    }
                }
                else {
                    if (button.name && button.name.startsWith("http")) {
                        button.action = button.name;
                    }
                    else {
                        // custom action is used so we need to trigger custom event on click
                        button.action = (editor) => {
                            dotNetObjectRef.invokeMethodAsync('NotifyCustomButtonClicked', button.name, button.value).then(null, function (err) {
                                throw new Error(err);
                            });
                        }
                    }
                }

                // button without icon is not allowed
                if (!button.className) {
                    button.className = "fa fa-question";
                }
            }
        });
    }

    let nextFileId = 0;
    let imageUploadNotifier = {
        onSuccess: (e) => { },
        onError: (e) => { }
    };

    var mermaidInstalled = false;
    var hljsInstalled = false;

    if (typeof hljs !== 'undefined')
        hljsInstalled = true;

    if (typeof mermaid !== 'undefined') {
        mermaidInstalled = true;
        if (options.toolbar == undefined) {
            options.toolbar = [
                "bold", "italic", "heading", "|",
                "undo", "redo", "|", "code",
                {
                    name: "addMermaid",
                    action: renderMermaid,
                    className: "fas fa-pie-chart",
                    title: "Add Mermaid",
                },
                "|",
                {
                    name: "addAttention",
                    action: renderAttention,
                    className: "fas fas fa-exclamation-circle",
                    title: "Add Attention",
                },
                {
                    name: "addNote",
                    action: renderNote,
                    className: "fas fa-info-circle",
                    title: "Add Note",
                },
                {
                    name: "addTip",
                    action: renderTip,
                    className: "fas fa-lightbulb",
                    title: "Add Tip",
                },
                {
                    name: "addWarning",
                    action: renderWarning,
                    className: "fas fa-exclamation-triangle",
                    title: "Add Warning",
                },
                {
                    name: "addVideo",
                    action: renderVideo,
                    className: "fa-solid fa-video",
                    title: "Add video",
                },
                "|", "video", "|", "quote", "unordered-list", "ordered-list", "|", 
                "link", "image", "table", "|", "fullscreen",
                "preview", "|", "guide"
            ];
        }
    }

    const easyMDE = new EasyMDE({
        element: document.getElementById(elementId),
        hideIcons: options.hideIcons,
        showIcons: options.showIcons,
        valueUpdateMode: options.valueUpdateMode,
        renderingConfig: {
            singleLineBreaks: false,
            codeSyntaxHighlighting: false,
            markedOptions: {
                langPrefix: "",
                highlight: (code, lang) => {

                    if (lang === "mermaid" && mermaidInstalled) {
                        //const tempDiv = document.createElement("div");
                        //tempDiv.className = "mermaid-container";

                        //console.log("Rendering mermaid", code)
                        //try {
                        //    const svg = mermaid.render("mermaid0", code);
                        //    tempDiv.innerHTML = svg;
                        //}catch (err) {
                        //    tempDiv.innerHTML = "Error rendering mermaid chart";
                        //    console.error(err);
                        //}
                        //return tempDiv;

                        window.setTimeout(() => {
                            mermaid.run({
                                querySelector: `#${previewElementId} code.mermaid`,
                            });
                        });

                        return code;
                    }
                    else if (lang === "quot") {
                        return '<div class="me-quot">' + code + '</div>';
                    }
                    else if (lang === "att") {
                        return '<div class="me-alert callout attention"><p class="title">' +
                            '<span class="me-icon icon-attention"></span> Attention</p><p>' +
                            code + '</p></div>';
                    }
                    else if (lang === "tip") {
                        return '<div class="me-alert callout tip"><p class="title">' +
                            '<span class="me-icon icon-tip"></span> Tip</p><p>' +
                            code + '</p></div>';
                    }
                    else if (lang === "note") {
                        return '<div class="me-alert callout note"><p class="title">' +
                            '<span class="me-icon icon-note"></span> Note</p><p>' +
                            code + '</p></div>';
                    }
                    else if (lang === "warn") {
                        return '<div class="me-alert callout warning"><p class="title">' +
                            '<span class="me-icon icon-warning"></span> Warning</p><p>' +
                            code + '</p></div>';
                    }
                    else if (lang === "video") {
                        var videoCode = '<div class="video-container">';

                        if (code.includes("youtube.com") || code.includes("youtu.be"))
                            videoCode = videoCode + '<iframe width="560" height="315" src="' + code + '" frameborder="0" allow="accelerometer; autoplay; clipboard-write; encrypted-media; gyroscope; picture-in-picture" allowfullscreen></iframe>';
                        else if (code.includes("vimeo.com"))
                            videoCode = videoCode + '<iframe src="' + code + '" width="640" height="360" frameborder="0" allow="autoplay; fullscreen" allowfullscreen></iframe>';
                        else
                            videoCode = videoCode + '<video controls><source src="' + code + '" type="video/mp4"></video>';

                        videoCode = videoCode + '</div>';
                        return videoCode;
                    }
                    else if (lang && hljsInstalled) {
                        const language = hljs.getLanguage(lang) ? lang : 'plaintext';
                        let highlighted = hljs.highlight(code, { language }).value;

                        console.log("highlight, options", options);

                        highlighted += `<div class='tb-code-overlay'>`;
                        if (options.enableRunCode && options.runCodeLanguages) {

                            // Only show play button for some languages
                            const supportedLanguages = options.runCodeLanguages.split(' ');
                            if (supportedLanguages.indexOf(language) != -1) {

                                const wrapperCssClass = `tb-code-run-overlay-${parseInt(Math.random() * 100000)}`;
                                highlighted += `<div class='tb-code-run-overlay ${wrapperCssClass}'>
                                                    <svg width="24" height="24" viewBox="0 0 24 24"><path d=\"M0 0h24v24H0z\" fill=\"none\"/><path fill=\"currentColor\" d=\"M8 5v14l11-7z\"/></svg>
                                                </div>`;

                                // Register click event, but no callback function when rendered
                                window.setTimeout(() => {
                                    const selector = `.${wrapperCssClass}`;
                                    const buttonElement = document.querySelector(selector);
                                    buttonElement.addEventListener("click", () => {
                                        dotNetObjectRef.invokeMethodAsync("RunCodeInternal", lang, code);
                                    });
                                }, 1000);
                            }
                        }

                        if (options.enableCopyCodeToClipboard) {
                            const wrapperCssClass = `tb-code-clipboard-overlay-${parseInt(Math.random() * 100000)}`;
                            highlighted += `<div class='tb-code-clipboard-overlay ${wrapperCssClass}'>
                                                <svg width="24" height="24" viewBox="0 0 24 24" xmlns="http://www.w3.org/2000/svg">
                                                    <path fill-rule="evenodd" clip-rule="evenodd" d="M19.5 16.5L19.5 4.5L18.75 3.75H9L8.25 4.5L8.25 7.5L5.25 7.5L4.5 8.25V20.25L5.25 21H15L15.75 20.25V17.25H18.75L19.5 16.5ZM15.75 15.75L15.75 8.25L15 7.5L9.75 7.5V5.25L18 5.25V15.75H15.75ZM6 9L14.25 9L14.25 19.5L6 19.5L6 9Z" fill="currentColor"/>
                                                </svg>
                                            </div>`;

                            // Register click event, but no callback function when rendered
                            window.setTimeout(() => {
                                const selector = `.${wrapperCssClass}`;
                                const buttonElement = document.querySelector(selector);
                                buttonElement.addEventListener("click", async () => {
                                    await navigator.clipboard.writeText(code);
                                });
                            }, 1000);
                        }
                        highlighted += `</div>`;

                        console.log("returning highlighted", highlighted);
                        return highlighted;
                    }
                    console.log("returning code", code);
                    return code;
                }
            }
        },
        initialValue: options.value,
        sideBySideFullscreen: false,
        autoDownloadFontAwesome: options.autoDownloadFontAwesome,
        lineNumbers: options.lineNumbers,
        lineWrapping: options.lineWrapping,
        minHeight: options.minHeight,
        maxHeight: options.maxHeight,
        placeholder: options.placeholder,
        tabSize: options.tabSize,
        theme: options.theme,
        direction: options.direction,
        toolbar: options.toolbar,
        toolbarTips: options.toolbarTips,
        spellChecker: options.spellChecker,
        nativeSpellcheck: options.nativeSpellcheck,

        autosave: options.autoSave,

        uploadImage: options.uploadImage,
        imageMaxSize: options.imageMaxSize,
        imageAccept: options.imageAccept,
        imageUploadEndpoint: options.imageUploadEndpoint,
        imagePathAbsolute: options.imagePathAbsolute,
        imageCSRFToken: options.imageCSRFToken,
        imageTexts: options.imageTexts,
        imageUploadFunction: (file, onSuccess, onError) => {
            imageUploadNotifier.onSuccess = onSuccess;
            imageUploadNotifier.onError = onError;

            NotifyUploadImage(elementId, file, dotNetObjectRef);
        },

        errorMessages: options.errorMessages,
        errorCallback: (errorMessage) => {
            dotNetObjectRef.invokeMethodAsync("NotifyErrorMessage", errorMessage);
        }
    });

    easyMDE.codemirror.on("blur", function () {
        console.log("blur", easyMDE.options.valueUpdateMode);
        if (easyMDE.options.valueUpdateMode === 2) {
            notifyDotNetValueChanged();
        }
    });

    easyMDE.codemirror.on("change", function () {
        console.log("change", easyMDE.options.valueUpdateMode);
        if (easyMDE.options.valueUpdateMode === 1) {
            notifyDotNetValueChanged();
        }
    });

    function notifyDotNetValueChanged() {
        const text = easyMDE.value();
        if (text === null || text === undefined) {
            return;
        }

        // Blazor will disconnect if too large payload, so we fragment it
        const maxSize = 20000;

        try {
            if (text.length < maxSize || false) {
                console.log("UpdateInternalValue: " + text.length);
                dotNetObjectRef.invokeMethodAsync("UpdateInternalValue", text);
            } else {
                let remainingChars = text.length;
                let offset = 0;

                dotNetObjectRef.invokeMethodAsync("BeginAppendInternalValue");

                while (remainingChars > 0) {
                    let numChars = Math.min(remainingChars, maxSize);
                    let packet = text.substr(offset, numChars);

                    console.log("AppendInternalValue: " + numChars);
                    dotNetObjectRef.invokeMethodAsync("AppendInternalValue", packet);

                    offset += numChars;
                    remainingChars -= numChars;

                }
                dotNetObjectRef.invokeMethodAsync("EndAppendInternalValue");
            }
        } catch (err) {
            console.error("Error invoking dotnet");
        }
    }
    function renderAttention(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```att\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    function renderMermaid(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```mermaid\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    function renderNote(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```note\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    function renderTip(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```tip\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    function renderWarning(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```warn\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    function renderVideo(editor) {
        var cm = editor.codemirror;
        var output = '';
        var selectedText = cm.getSelection();
        var text = selectedText || '';

        output = '```video\r\n' + text + '\r\n```';
        cm.replaceSelection(output);
    }

    instances[elementId] = {
        dotNetObjectRef: dotNetObjectRef,
        elementId: elementId,
        previewElementId: previewElementId,
        editor: easyMDE,
        imageUploadNotifier: imageUploadNotifier
    };

    //console.log("easyMDE", easyMDE);

    if (options.preview === true) {
        setPreview(elementId, true);
    } else {
        setPreview(elementId, false);
    }
}

function allowResize(id) {
    $(document).ready(function () {
        //$('#' + id + '.resizable:not(.processed)').TextAreaResizer();
    });
}

function deleteAutoSave(id) {
    $.each(localStorage, function (key, str) {
        if (key.startsWith('smde_' + id)) {
            localStorage.removeItem("smde_" + id);
        }
    });
}

function deleteAllAutoSave() {
    $.each(localStorage, function (key, str) {
        if (key.startsWith('smde_')) {
            localStorage.removeItem(key);
        }
    });
}

/**
 * Toggles italic
 * @param {any} elementId
 */
function toggleItalic(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleItalic();
    }
}

/**
 * Toggles strike-through
 * @param {any} elementId
 */
function toggleStrikethrough(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleStrikethrough();
    }
}

/**
 * Toggles code block
 * @param {any} elementId
 */
function toggleCodeBlock(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleCodeBlock();
    }
}

/**
 * 
 * @param {any} elementId
 */
function drawLink(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.drawLink();
    }
}

/**
 * 
 * @param {any} elementId
 */
function toggleOrderedList(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleOrderedList();
    }
}

/**
 * 
 * @param {any} elementId
 */
function toggleUnorderedList(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleUnorderedList();
    }
}

/**
 * 
 * @param {any} elementId
 */
function drawImage(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.drawImage();
    }
}

/**
 * 
 * @param {any} elementId
 */
function insertText(elementId, text) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {

        var cm = instance.editor.codemirror;
        cm.replaceSelection(text);
    }
}

/**
 * 
 * @param {any} elementId
 */
function drawTable(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.drawTable();
    }
}

/**
 * 
 * @param {any} elementId
 */
function drawHorizontalRule(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.drawHorizontalRule();
    }
}

/**
* Undo
* @param {any} elementId
*/
function undo(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.undo();
    }
}


/**
* Redo
* @param {any} elementId
*/
function redo(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.redo();
    }
}

/**
 * clean block (remove headline, list, blockquote code, markers)
 * @param {any} elementId
 */
function cleanBlock(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.cleanBlock();
    }
}

/**
 * Toggles header smaller
 * normal -> h1 -> h2 -> h3 -> h4 -> h5 -> h6 -> normal
 * @param {any} elementId
 */
function toggleHeadingSmaller(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeadingSmaller();
    }
}


/**
 * Toggles header smaller
 * normal -> h6 -> h5 -> h4 -> h3 -> h2 -> h1 -> normal
 * @param {any} elementId
 */
function toggleHeadingBigger(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeadingBigger();
    }
}

/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading1(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading1();
    }
}
    
/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading2(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading2();
    }
}

/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading3(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading3();
    }
}

/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading4(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading4();
    }
}

/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading5(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading5();
    }
}

/**
 * Toggles header
 * @param {any} elementId
 */
function toggleHeading6(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleHeading6();
    }
}

/**
 * Toggles undordered list
 * @param {any} elementId
 */
function toggleUnorderedList(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleUnorderedList();
    }
}

/**
 * Toggles dordered list
 * @param {any} elementId
 */
function toggleOrderedList(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleOrderedList();
    }
}

/**
 * Toggles bold
 * @param {any} elementId
 */
function toggleBold(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleBold();
    }
}

async function setFullScreen(rootElementId, wantedFullscreen) {
    const element = document.getElementById(rootElementId);
    if (wantedFullscreen) {
        try {
            await element.requestFullscreen();
        } catch (err) {
            console.error(`Error attempting to enable full-screen mode: ${err.message}`);
        }
    } else {
        document.exitFullscreen();
    }
}

/**
 * Toggles fullscreen
 * @param {any} elementId
 */
async function toggleFullScreen(rootElementId) {
    const element = document.getElementById(rootElementId);
    if (!document.fullscreenElement) {
        await setFullScreen(rootElementId, false);
    } else {
        await setFullScreen(rootElementId, true);
    }
}

/**
 * Toggles side-by-side for the specified element
 * @param {string} elementId
 */
function toggleSideBySide(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toggleSideBySide();
    }
}
/**
 * Toggles preview for the specified element
 * @param {string} elementId
 */
function togglePreview(elementId) {
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.togglePreview();
    }
}

/**
 * Acticates or deactives preview mode for the specified element
 * @param {string} elementId
 * @param {boolean} wantedState
 */
function setPreview(elementId, wantedState) {
    const instance = _instances[elementId];

    console.log("set preview " + wantedState);

    if (instance && instance.editor) {

        //const isActive = instance.editor.isPreviewActive();

        if (instance.previewElementId) {
            const rootElement = document.querySelector(`#root-${instance.elementId} > .EasyMDEContainer`);
            const previewElement = document.getElementById(instance.previewElementId);
            if (wantedState) {
                const value = instance.editor.value();
                console.log("renderpreview", value);
                try {
                    const previewHtml = instance.editor.options.previewRender(value);

                    //console.log(">> Rendered preview", previewHtml);
                    instance.previewEnabled = true;

                    previewElement.innerHTML = previewHtml;
                    previewElement.style.display = "block";
                    rootElement.style.display = "none";
                } catch (err) {
                    console.error(err);
                }
            } else {
                //console.log(">> Disabled preview", instance.editor.value());
                previewElement.innerHTML = "";
                previewElement.style.display = "none";
                rootElement.style.display = "grid";
                instance.previewEnabled = false;
                setValue(elementId, instance.editor.value());
            }
        }

    //    if (isActive != wantedState) {
    //        instance.editor.togglePreview();
    //    }
    }
}

function destroy(element, elementId) {
    const instances = _instances || {};

    // Fix for #54: Remove from DOM when MarkdownEditor is disposed
    const instance = _instances[elementId];
    if (instance && instance.editor) {
        instance.editor.toTextArea();
    }

    delete instances[elementId];
}

function setValue(elementId, value) {
    const instance = _instances[elementId];

    console.log("setValue()");

    if (instance) {
        instance.isChangingValue = true;
        instance.editor.value(value);
        instance.isChangingValue = false;

        if (instance.previewEnabled) {
            console.log("SetValue in preview");
            setPreview(elementId, true);
        } else {
            console.log("SetValue, not in preview");
        }
    }
}
function appendValue(elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        value = instance.editor.value() + value;
        instance.isChangingValue = true;
        instance.editor.value(value);
        instance.isChangingValue = false;

        if (instance.previewEnabled) {
            setPreview(elementId);
        }
    }
}

function setInitValue(elementId, value) {
    const instance = _instances[elementId];

    if (instance) {
        instance.editor.value(value);
    }
}

function getValue(elementId) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.editor.value();
    }

    return null;
}

function notifyImageUploadSuccess(elementId, imageUrl) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.imageUploadNotifier.onSuccess(imageUrl);
    }
}

function notifyImageUploadError(elementId, errorMessage) {
    const instance = _instances[elementId];

    if (instance) {
        return instance.imageUploadNotifier.onError(errorMessage);
    }
}

function _arrayBufferToBase64(buffer) {
    var binary = '';
    var bytes = new Uint8Array(buffer);
    var len = bytes.byteLength;
    for (var i = 0; i < len; i++) {
        binary += String.fromCharCode(bytes[i]);
    }
    return window.btoa(binary);
}

async function NotifyUploadImage(elementId, file, dotNetObjectRef) {
    var arrBuffer = await file.arrayBuffer();

    const a = await arrBuffer;
    var arrBf = _arrayBufferToBase64(a);

    var fileEntry = {
        lastModified: new Date(file.lastModified).toISOString(),
        name: file.name,
        size: file.size,
        type: file.type,
        contentbase64: arrBf
    };

    dotNetObjectRef.invokeMethodAsync('NotifyImageUpload', fileEntry).then(null, function (err) {
        throw new Error(err);
    });

    dotNetObjectRef.invokeMethodAsync('UploadFile', fileEntry).then(r => {
        if (!r || r.length === 0)
            notifyImageUploadError(elementId, "Upload error");
        else
            notifyImageUploadSuccess(elementId, r);
    });
}

const meLoadCSS = function (name, url) {
    if (document.getElementById(name))
        return;

    return new Promise(function (resolve, reject) {
        const link = document.createElement('link');
        link.rel = "stylesheet";
        link.type = "text/css";
        link.href = sourceUrl;

        link.addEventListener('load', function () {
            // The script is loaded completely
            resolve(true);
        });

        document.head.appendChild(script);
    });
};

const meLoadScript = function (name, url) {
    if (document.getElementById(name))
        return;

    return new Promise(function (resolve, reject) {
        const script = document.createElement('script');
        script.src = url;
        script.id = name;

        script.addEventListener('load', function () {
            // The script is loaded completely
            resolve(true);
        });

        document.head.appendChild(script);
    });
};

window.renderMermaidDiagram = async () => {
    const collection = document.getElementsByTagName("code");

    for (let i = 0; i < collection.length; i++) {
        if (collection[i].classList.contains("mermaid")) {
            try {
                console.log(collection[i].innerHTML);
                var svg = await mermaid.render("theGraph", collection[i].innerHTML);
                collection[i].innerHTML = svg;
            } catch (error) {
                collection[i].innerHTML = "Invalid syntax. " + error; // Display error message
            }
        }
    }
} 