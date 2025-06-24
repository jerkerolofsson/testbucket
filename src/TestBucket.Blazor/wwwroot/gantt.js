class GanttTask {
    constructor(data) {
        this.data = data;
        this.shapes = {rect: null, links: []};
        this.startDate = null;
        this.endDate = null;

        this.colors = {fill: {}, stroke: {}};
        this.colors.fill.back = data.colors?.fill?.back;// ?? "#3465bd";
        this.colors.fill.progress = data.colors?.fill?.progress;// ?? "#6495ed";
        this.colors.fill.hover = data.colors?.fill?.hover;// ?? "#4475cd";
        this.colors.fill.progressHover = data.colors?.fill?.progressHover;// ?? "#74A5Fd";
        this.colors.fill.edge = data.colors?.fill?.edge;// ?? "#14459d";
        this.colors.stroke.link = data.colors?.stroke?.link;// ?? "#84B5FF";

        if (data.start) {
            this.startDate = new Date(data.start);
            this.startEpochs = this.startDate.getTime();
        }
        if (data.end) {
            this.endDate = new Date(data.end);
            this.endEpochs = this.endDate.getTime();
        }
        this.startDate ??= this.endDate;
        this.endDate ??= this.startDate;
        this.startEpochs ??= this.endEpochs;
        this.endEpochs ??= this.startEpochs;

	if(!this.startEpochs) {
		throw new Exception("No date set for task");
	}

	this.range = this.endEpochs - this.startEpochs;
    }
}

class GanntChart {
    constructor(id, selector) {
        this.id = id;
        this.selector = selector;
        this.container = document.querySelector(selector);
        if (!this.container) {
            console.error(`Container with selector ${selector} not found.`);
            return;
        }

        this.options = {};

        this.options.arrowSize = 7;
        this.options.arrowPadding = 10;
        this.options.taskHeight = 25;
        this.options.edgeSize = 5;
        this.options.textOffsetX = 5;
        this.options.textOffsetY = 2;

        this.viewBoxWidth = 1920/2;
        this.viewBoxHeight = 1080/2;
        this.tasks = [];

        this.container.classList.add('gantt-container');
    }

    #reset() {

        if(!this.ganttChart) {

            // Clear existing content
            this.container.innerHTML = "";

            // Create a new Gantt chart element
            this.ganttChart = document.createElementNS("http://www.w3.org/2000/svg", "svg");

            // Add view box attribute to ganttChart
            //this.ganttChart.setAttribute('preserveAspectRatio', "none");
            this.ganttChart.setAttribute('width', "100%");
            this.ganttChart.setAttribute('height', "100%");
            this.ganttChart.setAttribute('viewBox', `0 0 ${this.viewBoxWidth} ${this.viewBoxHeight}`);
            this.container.appendChild(this.ganttChart);

            document.addEventListener("mouseup", (event) => { this.#onMouseUp(event) });
            this.ganttChart.addEventListener("mousedown", (event) => { this.#onMouseDown(event) });
            this.ganttChart.addEventListener("mousemove", (event) => { this.#onMouseMove(event) });
        } else {
            this.ganttChart.innerHTML = "";
        }
    }

    #addTaskData() {
        this.tasks = [];
        this.minDate = null;
        this.maxDate = null;
        this.data.items.forEach(task => {
            const taskData = new GanttTask(task);

            // Determine date range for all items
            if (taskData.endDate) {
                if (!this.maxDate) {
                    this.maxDate = taskData.startDate;
                    this.maxDate = taskData.endDate;
                }
            }
            if (taskData.startDate) {
                if (!this.minDate) {
                    this.minDate = taskData.startDate;
                    this.minDate = taskData.startDate;
                }
            }
            if (taskData.startDate < this.minDate) {
                this.minDate = taskData.startDate;
            }
            if (taskData.endDate < this.minDate) {
                this.minDate = taskData.endDate;
            }
            if (taskData.startDate > this.maxDate) {
                this.maxDate = taskData.startDate;
            }
            if (taskData.endDate > this.maxDate) {
                this.maxDate = taskData.endDate;
            }

            this.tasks.push(taskData);
        });

    	if(this.minDate) {
    		this.minEpochs = this.minDate.getTime();
    		this.maxEpochs = this.maxDate.getTime();
    		this.range = this.maxEpochs - this.minEpochs;
    	}
    }

    createLinks() {
        const links = [];
        for(const task of this.tasks) {
            if(task.data.dependentOn) {
                for (const taskId of task.data.dependentOn) {
                    const matches = [...this.tasks.filter(x => x.data.id == taskId)];
                    for (const task2 of matches) {
                        links.push({task1: task, task2: task2});
                    }
                }
            }
        }
        for (const link of links) {
            this.createLink(link.task1, link.task2);
        }
    }

    createLink(task1, task2) {

        const spacing = this.options.arrowPadding;

        const pathLine = document.createElementNS("http://www.w3.org/2000/svg", "path");
        pathLine.setAttribute("data-type", "link-line");
        if (task1.colors.stroke.link) {
            pathLine.setAttribute("stroke", task1.colors.stroke.link);
        }

        let d = `M${task1.x} ${task1.y + task1.height/2} `;

        const yDelta = task2.y - task1.y;
        const xDelta = task2.right - task1.left;

        if(xDelta > -spacing*2) {

            d += `h ${-spacing} `; // horizontal delta
            d += `v ${yDelta/2} `; // vertical delta1
            d += `h ${xDelta + spacing + spacing} `; // horizontal delta
            d += `v ${yDelta/2} `; // vertical delta2
            d += `h ${-spacing} `; // horizontal delta
        } else {

            d += `h ${xDelta+spacing} `; // horizontal delta
            d += `v ${yDelta} `; // vertical
            d += `h ${-spacing} `; // horizontal delta
        }
        pathLine.setAttribute("d", d);

        // Arrow
        let arrowSize = this.options.arrowSize;
        const pathArrow = document.createElementNS("http://www.w3.org/2000/svg", "path");
        pathArrow.setAttribute("data-type", "link-arrow");
        pathArrow.setAttribute("fill", task1.colors.stroke.link);
        const arrowX = task1.left;
        const arrowY = task1.top + task1.height / 2;
        let dArrow = `M${arrowX} ${arrowY} `;
        dArrow += `L${arrowX - arrowSize} ${arrowY - arrowSize / 2} `;
        dArrow += `L${arrowX - arrowSize} ${arrowY + arrowSize / 2} `;
        //dArrow += `L${arrowX} ${arrowY} `;
        pathArrow.setAttribute("d", dArrow);

        this.ganttChart.insertBefore(pathLine, this.ganttChart.firstChild);
        this.ganttChart.insertBefore(pathArrow, this.ganttChart.firstChild);
        task1.shapes.links.push(pathLine);
        task1.shapes.links.push(pathArrow);
    }

    updateLinks() {
        this.removeLinks();
        this.createLinks();
    }

    removeLinks() {
        for(const task of this.tasks) {
            for(const link of task.shapes.links) {
                link.remove();
            }
            task.shapes.links = [];
        }
    }

    updateData(data) {
        this.data = data;

        this.#reset();
        this.#addTaskData();

        // Populate the Gantt chart with data
        const taskHeight = this.options.taskHeight;
        const rowMargin = 10;

        const rowHeight = taskHeight + rowMargin;

        let y = 0;
        for (const task of this.tasks) {

            task.colors ??= data.colors;

            const relativeEpochs = (task.startEpochs - this.minEpochs);
            const x = relativeEpochs * this.viewBoxWidth / this.range;
            const width = task.range * this.viewBoxWidth / this.range;

            // console.log(`${task.data.name}: task.relativeEpochs=${relativeEpochs}, task.range=${task.range}, this.range=${this.range}, x=${x}, fill=${task.fill}`);

            const taskElement = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            taskElement.setAttribute('data-id', task.data.id);
            taskElement.setAttribute("data-type", "task");
            taskElement.setAttribute('rx', 2);
            taskElement.setAttribute('x', x);
            taskElement.setAttribute('y', y);
            taskElement.setAttribute('width', width);
            taskElement.setAttribute('height', taskHeight);
            if (task.data.class) {
                taskElement.setAttribute('class', task.data.class);
            }
            if (task.data.style) {
                taskElement.setAttribute('style', task.data.style);
            }
            if (task.colors.fill.back) {
                taskElement.setAttribute('fill', task.colors.fill.back);
            }
            task.left = task.x = x;
            task.top = task.y = y;
            task.width = width;
            task.height = taskHeight;
            task.bottom = task.y + taskHeight;
            task.right = task.x + width;
            task.shapes.rect = taskElement;
            this.ganttChart.appendChild(taskElement);

            // Show progress
            if(task.data.progress !== undefined) {

                const progressWidth = width * task.data.progress;

                const progressElement = document.createElementNS("http://www.w3.org/2000/svg", "rect");
                progressElement.setAttribute("data-type", "progress");
                progressElement.setAttribute('rx', 2);
                progressElement.setAttribute('x', x);
                progressElement.setAttribute('y', y);
                progressElement.setAttribute('width', progressWidth);
                progressElement.setAttribute('height', task.height);
                if (task.colors.fill.progress) {
                    progressElement.setAttribute('fill', task.colors.fill.progress);
                }
                task.shapes.progress = progressElement;
                this.ganttChart.appendChild(progressElement);
            }

            // Edges to resize the task
            const edgeLeft = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            edgeLeft.setAttribute("data-type", "edge-left");
            edgeLeft.setAttribute('rx', 2);
            edgeLeft.setAttribute('x', task.left);
            edgeLeft.setAttribute('y', task.top);
            edgeLeft.setAttribute('width', this.options.edgeSize);
            edgeLeft.setAttribute('height', task.height);
            if(task.colors.fill.edge) {
                edgeLeft.setAttribute('fill', task.colors.fill.edge);
            }
            edgeLeft.setAttribute('opacity', "0");
            edgeLeft.style.cursor = "ew-resize";
            task.shapes.edgeLeft = edgeLeft;
            this.ganttChart.appendChild(edgeLeft);

            const edgeRight = document.createElementNS("http://www.w3.org/2000/svg", "rect");
            edgeRight.setAttribute("data-type", "edge-right");
            edgeRight.setAttribute('rx', 2);
            edgeRight.setAttribute('x', task.right - this.options.edgeSize);
            edgeRight.setAttribute('y', task.top);
            edgeRight.setAttribute('width', this.options.edgeSize);
            edgeRight.setAttribute('height', task.height);
            if (task.colors.fill.edge) {
                edgeRight.setAttribute('fill', task.colors.fill.edge);
            }
            edgeRight.setAttribute('opacity', "0");
            edgeRight.style.cursor = "ew-resize";
            task.shapes.edgeRight = edgeRight;
            this.ganttChart.appendChild(edgeRight);

            // Text label
            const textElement = document.createElementNS("http://www.w3.org/2000/svg", "text");
            textElement.setAttribute('x', x + this.options.textOffsetX);
            textElement.setAttribute('y', y + this.options.textOffsetY);
            textElement.setAttribute('width', width);
            textElement.setAttribute('height', taskHeight);
            textElement.innerHTML = task.data.name;
            task.shapes.text = textElement;
            this.ganttChart.appendChild(textElement);


            // Text label for progress
            if(task.data.progress !== undefined) {
                const textProgressElement = document.createElementNS("http://www.w3.org/2000/svg", "text");
                textProgressElement.setAttribute('data-type', "progress-label");
                textProgressElement.setAttribute('x', x + this.options.textOffsetX);
                textProgressElement.setAttribute('y', y + task.height - this.options.textOffsetY);
                textProgressElement.setAttribute('width', width);
                textProgressElement.setAttribute('height', taskHeight);
                textProgressElement.innerHTML = Math.round(task.data.progress*100) + "%";
                task.shapes.textProgress = textProgressElement;
                this.ganttChart.appendChild(textProgressElement);
            }

            y += rowHeight;
        }

        this.createLinks();
    }

    #cursorPoint(evt){
        const pt = this.ganttChart.createSVGPoint();
        pt.x = evt.clientX; 
        pt.y = evt.clientY;
        return pt.matrixTransform(this.ganttChart.getScreenCTM().inverse());
    }
    #onMouseDown(event) {
        this.dragProgress = false;
        if(this.hoverLeftEdge || this.hoverRightEdge) {
            this.isDragging = true;
        } else {
            if(this.hoverTask?.data?.progress !== undefined) {
                const task = this.hoverTask;
                const pt = this.#cursorPoint(event);
                const progress = (pt.x-task.x) / task.width;
                this.setProgress(task, progress);
                this.isDragging = true;
                this.dragProgress = true;
                this.dragTask = task;
            }
        }
    }

    setProgress(task, progress) {
        if(progress < 0) {
            progress = 0;
        }
        if(progress > 1) {
            progress = 1;
        }
        if(task.shapes.progress) {
            task.shapes.progress.setAttribute("width", progress * task.width);
        }
        if(task.shapes.textProgress) {
            const progressText = Math.round(progress*100.0) + "%";
            task.shapes.textProgress.innerHTML = progressText;
        }
        if(task.data.progress) {
            task.data.progress = progress;
        }
    }

    #updateTaskFromEdges() {
        const task = this.dragTask;
        if(this.hoverRightEdge) {
            let x = parseInt(task.shapes.edgeRight.getAttribute("x"));
            if (task.left+5 > x) {
                x = task.left+5;
            }
            task.right = x;
            const width = task.right - task.left;
            task.width = width;
            task.shapes.progress?.setAttribute("width", width * task.data.progress);
            task.shapes.rect.setAttribute("width", width);
            task.shapes.edgeRight.setAttribute("x", x - this.options.edgeSize);
        }
        else if(this.hoverLeftEdge) {
            let x = parseInt(task.shapes.edgeLeft.getAttribute("x"));
            if (task.right-5 < x) {
                x = task.right-5;
            }

            task.left = task.x = x;
            const width = task.right - x;
            task.width = width;
            task.shapes.rect.setAttribute("x", x);
            task.shapes.progress?.setAttribute("x", x);
            task.shapes.progress?.setAttribute("width", width * task.data.progress);
            task.shapes.rect.setAttribute("width", width);
            task.shapes.edgeLeft.setAttribute("x", x);

            task.shapes.text.setAttribute("x", x + this.options.textOffsetX);
            if(task.shapes.textProgress) {
                task.shapes.textProgress.setAttribute("x", x + this.options.textOffsetX);
            }
        }
        this.updateLinks();
    }

    #onMouseUp(event) {
        if(this.isDragging) {
            if(this.isDragging && this.dragTask) {
                this.#updateTaskFromEdges();
            }
            this.isDragging = false;
            this.dragProgress = false;
        }
    }

    #onMouseMove(event) {
        const pt = this.#cursorPoint(event);
        const nx = pt.x;
        const ny = pt.y;

        // console.log(`nx=${nx}`, pt);

        if(this.isDragging && this.dragTask) {
            if(this.dragProgress)
            {
                const task = this.dragTask;
                const progress = (pt.x-task.x) / task.width;
                this.setProgress(task, progress);
            }
            else if(this.hoverLeftEdge) {
                this.dragTask.shapes.edgeLeft.setAttribute("x", pt.x);
                this.#updateTaskFromEdges();
            }
            else if(this.hoverRightEdge) {
                this.dragTask.shapes.edgeRight.setAttribute("x", pt.x);
                this.#updateTaskFromEdges();
            }
            return;
        }

        this.hoverTask = null;
        this.hoverLeftEdge = false;
        this.hoverRightEdge = false;
        this.hoverProgress = false;

        const edgeSize = this.options.edgeSize;

        for (const task of this.tasks) {

             //console.log(`${task.left} ${nx} ${task.left+edgeSize}`);

            if (nx >= task.left && nx < task.right && ny >= task.top && ny < task.bottom) {

                this.hoverTask = task;
                this.dragTask = task;
                task.shapes.rect.setAttribute("data-hover", "true");
                if (task.shapes.progress) {
                    task.shapes.progress.setAttribute("data-hover", "true");
                }

                // Over right edge
                if (nx >= task.right - edgeSize && nx < task.right && ny >= task.top && ny < task.bottom) {
                    // Hover right
                    this.hoverRightEdge = true;
                    task.shapes.edgeRight.setAttribute("opacity", 1);
                    task.shapes.edgeLeft.setAttribute("opacity", 0);
                }
                else if (nx >= task.left && nx < task.left + edgeSize && ny >= task.top && ny < task.bottom) {
                    // Hover left
                    this.hoverLeftEdge = true;
                    task.shapes.edgeRight.setAttribute("opacity", 0);
                    task.shapes.edgeLeft.setAttribute("opacity", 1);
                } else {
                    task.shapes.edgeRight.setAttribute("opacity", 0);
                    task.shapes.edgeLeft.setAttribute("opacity", 0);
                }

                // Highlight task under mouse position
                if (task.colors.fill.hover) {
                    task.shapes.rect.setAttribute("fill", task.colors.fill.hover);
                }
                if (task.shapes.progress && task.colors.fill.progressHover) {
                    task.shapes.progress.setAttribute("fill", task.colors.fill.progressHover);
                }
            } else {
                task.shapes.rect.removeAttribute("data-hover");

                task.shapes.edgeRight.setAttribute("opacity", 0);
                task.shapes.edgeLeft.setAttribute("opacity", 0);

                if (task.colors.fill.back) {
                    task.shapes.rect.setAttribute("fill", task.colors.fill.back);
                }
                if (task.shapes.progress) {
                    task.shapes.progress.removeAttribute("data-hover");
                    if (task.shapes.progress && task.colors.fill.progress) {
                        task.shapes.progress.setAttribute("fill", task.colors.fill.progress);
                    }
                }
            }
        }
    }
}

window.blazorGanttCharts = {};

function initGantt(id, selector) {

    const instance = new GanntChart(id, selector);
    window.blazorGanttCharts[id] = instance;
    return instance;
}

function updateGantt(id, data) {

    if (window.blazorGanttCharts[id]) {
        window.blazorGanttCharts[id].updateData(data)
    }
}

