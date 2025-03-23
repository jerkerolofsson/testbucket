const _splitterInstances = [];


function splitterInitialize(elementId, options) {
    options ??= {};
    options.selector = `#${elementId}`
    _splitterInstances[elementId] = new Splitter(options);
}

function splitterDestroy(elementId) {
    if (_splitterInstances[elementId]) {
        const splitter = _splitterInstances[elementId];
        splitter.destroy();

        delete _splitterInstances[elementId];
    }
}

class Splitter {

    constructor(options) {
        this.options = options;
        this.direction = options?.direction ?? "horizontal";
        this.dimensions = options?.dimension ?? "50%";
        this.selector = options.selector;
        this.destroyed = false;
        this.bind();
    }

    destroy() {
        this.destroyed = true;
        this.splitter.removeEventListener("mousedown", this.boundOnMouseDown);
        window.removeEventListener("mousemove", this.boundOnMouseMove);

        window.removeEventListener("drag", this.boundDrag);
        this.splitter.removeEventListener("dragstart", this.boundDragStart);

        this.splitter.removeEventListener("touchstart", this.boundTouchStart);
        window.removeEventListener("touchmove", this.boundTouchMove);

        window.removeEventListener("touchcancel", this.boundEnd);
        window.removeEventListener("dragend", this.boundEnd);
        window.removeEventListener("mouseup", this.boundEnd);
        window.removeEventListener("touchend", this.boundEnd);
    }

    #beginDrag(e) {
        if (this.destroyed) return;

        console.log(e);

        this.dragging = true;
        if (e.touches?.length > 0) {
            this.dragStartX = e.touches[0].pageX;
            this.dragStartY = e.touches[0].pageY;
        } else {
            this.dragStartX = e.pageX;
            this.dragStartY = e.pageY;
        }

        const sibling = this.startContent;

        if (!this.dragW) {
            this.dragStartW = sibling.clientWidth;
        } else {
            this.dragStartW = this.dragW;
        }
        if (!this.dragH) {
            this.dragStartH = sibling.clientHeight;
        } else {
            this.dragStartH = this.dragH;
        }
        e.preventDefault();
    }

    #onDrag(e, sourceType) {
        if (this.destroyed) return;
        const selector = this.selector;

        if (this.dragging === true) {
            let pageX, pageY;
            if (e.touches && e.touches.length > 0) {
                pageX = e.touches[0].pageX;
                pageY = e.touches[0].pageY;
            } else {
                pageX = e.pageX;
                pageY = e.pageY;
            }

            let deltaX = pageX - this.dragStartX;
            let deltaY = pageY - this.dragStartY;

            // console.log(`#onDrag this.dragging=${this.dragging}, deltaX=${deltaX}, deltaY=${deltaY}, this.dragStartX=${this.dragStartX}, pageX=${pageX}`);

            let left = this.startContent;
            let right = this.endContent;
            let parent = this.container;

            let containerW = parent.clientWidth;
            let containerH = parent.clientHeight;

            let w = this.dragStartW + deltaX;
            let h = this.dragStartH + deltaY;

            let margins = this.options.minStartContentSize ?? 32;
            if (w < margins) {
                w = margins;
            }
            if (h < margins) {
                h = margins;
            } 

            this.dragW = w;
            this.dragH = h;

            //console.log(`containerW=${containerW}, deltaX=${deltaX}, deltaY=${deltaY}`);

            let leftPercent = w * 100 / containerW;
            let rightPercent = 100 - leftPercent;

            if (this.direction == 'horizontal') {
                this.startContent.style.width = `${w}px`;
                this.container.style["grid-template-columns"] = `${w}px var(--bar-size, 5px) auto`;
            } else {
                this.startContent.style.height = `${h}px`;
                this.container.style["grid-template-rows"] = `${h}px var(--bar-size, 5px) auto`;

            }

            if (sourceType != "touch") {
                e.preventDefault();
            }
        }
    }

    #onMouseMove(e) {
        this.#onDrag(e, "mouse");
    }

    #dragStart(e) {
        this.#beginDrag(e, "drag");
    }

    #onTouchStart(e) {
        this.#beginDrag(e, "touch");
    }

    #onMouseDown(e) {
        if (this.destroyed) return;
        e.preventDefault();

        console.log("onMouseDown", e);
        this.#beginDrag(e, "mouse");
    }

    #onTouchMove(e) {
        this.#onDrag(e, "touch");
    }

    #end() {
        console.log("RESIZE END");
        this.dragging = false;
    }

    bind() {

        this.container = document.querySelector(this.options.selector);
        this.splitter = document.querySelector(`${this.options.selector} > .tb-splitter-bar`);
        this.startContent = document.querySelector(`${this.options.selector} > .tb-splitter-content-start`);
        this.endContent = document.querySelector(`${this.options.selector} > .tb-splitter-content-end`);

        this.container.style.display = "grid";
        if (this.direction == 'horizontal') {
            this.startContent.style.width = "100%";
            this.container.style["grid-template-columns"] = `${this.dimensions} var(--bar-size, 5px) auto`;
        } else {
            this.startContent.style.height = "100%";
            this.container.style["grid-template-rows"] = `${this.dimensions} var(--bar-size, 5px) auto`;
        }

        this.boundOnMouseDown = (e) => { this.#onMouseDown(e); }
        this.boundOnMouseMove = (e) => { this.#onMouseMove(e); }
        this.boundDrag = (e) => { this.#onDrag(e); }
        this.boundDragStart = (e) => { this.#dragStart(e); }
        this.boundTouchStart = (e) => { this.#onTouchStart(e); }
        this.boundTouchMove = (e) => { this.#onTouchMove(e); }
        this.boundEnd = (e) => { this.#end(e); }

        this.splitter.addEventListener("mousedown", this.boundOnMouseDown);
        window.addEventListener("mousemove", this.boundOnMouseMove);

        window.addEventListener("drag", this.boundDrag);
        this.splitter.addEventListener("dragstart", this.boundDragStart);

        this.splitter.addEventListener("touchstart", this.boundTouchStart);
        window.addEventListener("touchmove", this.boundTouchMove);

        window.addEventListener("touchcancel", this.boundEnd);
        window.addEventListener("dragend", this.boundEnd);
        window.addEventListener("mouseup", this.boundEnd);
        window.addEventListener("touchend", this.boundEnd);
    }
}
