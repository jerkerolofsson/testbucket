import './d3.v7.min.js'

let graph = null;
export function initializeStates(resourcesInterop) {
    graph = new StatesGraph(resourcesInterop);
    graph.resize();

    const observer = new ResizeObserver(function () {
        graph.resize();
    });

    for (const child of document.getElementsByClassName('state-graph')) {
        observer.observe(child);
    }
}

export function updateGraph(states) {

    console.log("updateGraph", states);

    if (graph) {
        try {
            graph.update(states);
        } catch (e) {
            console.error(e);
        }
    }
}

export function updateSelected(name) {
    if (graph) {
        graph.switchTo(name);
    }
}

class StatesGraph {
    constructor(resourcesInterop) {
        this.resources = [];
        this.resourcesInterop = resourcesInterop;
        this.openContextMenu = false;

        this.circleSize = 15;

        this.states = [];
        this.links = [];

        this.svg = d3.select('.state-graph');
        this.baseGroup = this.svg.append("g");

        // Enable zoom + pan
        // https://www.d3indepth.com/zoom-and-pan/
        // scaleExtent limits zoom to reasonable values
        this.zoom = d3.zoom().scaleExtent([0.2, 4]).on('zoom', (event) => {
            this.baseGroup.attr('transform', event.transform);
        });
        this.svg.call(this.zoom);

        // simulation setup with all forces
        this.linkForce = d3
            .forceLink()
            .id(function (link) { return link.id })
            .strength(function (link) { return 1 })
            .distance(150);

        this.simulation = d3
            .forceSimulation()
            .force('link', this.linkForce)
            .force('charge', d3.forceManyBody().strength(-800))
            .force("collide", d3.forceCollide(110).iterations(10))
            .force("x", d3.forceX().strength(0.2))
            .force("y", d3.forceY().strength(0.2));

        // Drag start is trigger on mousedown from click.
        // Only change the state of the simulation when the drag event is triggered.
        var dragActive = false;
        var dragged = false;
        this.dragDrop = d3.drag().on('start', (event) => {
            dragActive = event.active;
            event.subject.fx = event.subject.x;
            event.subject.fy = event.subject.y;
        }).on('drag', (event) => {
            if (!dragActive) {
                this.simulation.alphaTarget(0.1).restart();
                dragActive = true;
            }
            dragged = true;
            event.subject.fx = event.x;
            event.subject.fy = event.y;
        }).on('end', (event) => {
            if (dragged) {
                this.simulation.alphaTarget(0);
                dragged = false;
            }
            event.subject.fx = null;
            event.subject.fy = null;
        });

        var defs = this.svg.append("defs");
        this.createArrowMarker(defs, "arrow-normal", "arrow-normal", this.circleSize, this.circleSize, this.circleSize+1);

        this.linkElementsG = this.baseGroup.append("g").attr("class", "links").attr("stroke", "#99F");
        this.nodeElementsG = this.baseGroup.append("g").attr("class", "state");

        this.initializeButtons();
    }

    initializeButtons() {
        d3.select('.graph-zoom-in').on("click", () => this.zoomIn());
        d3.select('.graph-zoom-out').on("click", () => this.zoomOut());
        d3.select('.graph-reset').on("click", () => this.resetZoomAndPan());
    }

    resetZoomAndPan() {
        this.svg.transition().call(this.zoom.transform, d3.zoomIdentity);
    }

    zoomIn() {
        this.svg.transition().call(this.zoom.scaleBy, 1.5);
    }

    zoomOut() {
        this.svg.transition().call(this.zoom.scaleBy, 2 / 3);
    }

    createArrowMarker(parent, id, className, width, height, x) {
        parent.append("marker")
            .attr("id", id)
            .attr("viewBox", "0 -5 10 10")
            .attr("refX", x)
            .attr("refY", 0)
            .attr("fill", "#99F")
            .attr("markerWidth", width)
            .attr("markerHeight", height)
            .attr("orient", "auto")
            .attr("markerUnits", "userSpaceOnUse")
            .attr("class", className)
            .append("path")
            .attr("d", 'M0,-5L10,0L0,5');
    }

    resize() {
        var container = document.querySelector(".state-graph");
        if (container) {
            var width = container.clientWidth;
            var height = Math.max(container.clientHeight - 50, 0);
            this.svg.attr("viewBox", [-width / 2, -height / 2, width, height]);
        }
    }

    switchTo(name) {
        this.selectedNode = this.states.find(node => node.name === name);
    }

    resourceEqual(r1, r2) {
        if (r1.id !== r2.id) {
            return false;
        }
        if (r1.type !== r2.type) {
            return false;
        }
        if (r1.color !== r2.color) {
            return false;
        }
        if (r1.name !== r2.name) {
            return false;
        }
       
        return true;
    }

    iconEqual(i1, i2) {
        if (i1.icon !== i2.icon) {
            return false;
        }
        if (i1.color !== i2.color) {
            return false;
        }
        if (i1.tooltip !== i2.tooltip) {
            return false;
        }

        return true;
    }

    hasChanged(existingResource, newResources) {
        if (!existingResource || newResources.length != existingResource.length) {
            return true;
        }

        for (var i = 0; i < newResources.length; i++) {
            if (!this.resourceEqual(newResources[i], existingResource[i], false)) {
                return true;
            }
        }

        return false;
    }

    updateStates(newStates) {
        const existingNodes = this.states || []; // Ensure nodes is initialized
        const updatedNodes = [];

        newStates.forEach(resource => {
            resource.id = resource.name;
            const existingNode = existingNodes.find(node => node.name === resource.name);

            if (existingNode) {
                // Update existing node without replacing it
                updatedNodes.push({
                    ...existingNode,
                    name: resource.name,
                    id: resource.id,
                    type: resource.type,
                });
            } else {
                // Add new resource
                updatedNodes.push({
                    name: resource.name,
                    id: resource.id,
                    type: resource.type,
                });
            }
        });

        this.states = updatedNodes;
    }

    update(states) {
        // Check if the overall structure of the graph has changed. i.e. nodes or links have been added or removed.
        var hasStructureChanged = this.hasChanged(this.states, states);
        this.states = states;
        this.updateStates(states);

        this.links = [];
        const stateNames = [...states.map(x => x.name)];

        for (var i = 0; i < states.length; i++) {
            var resource = states[i];

            resource.allowedStates ??= [];

            console.log("resource.allowedStates", resource.allowedStates);
            const allowedStates = [...resource.allowedStates.filter(x => stateNames.indexOf(x) != -1)];
            console.log("allowedStates", allowedStates);

            var resourceLinks = allowedStates
                .filter((refId) => {
                    return states.filter(r => r.id === refId);
                })
                .map((refId, index) => {
                    return {
                        id: `${resource.id}-${refId}`,
                        target: refId,
                        source: resource.id,
                        strength: 0.7
                    };
                });

            this.links.push(...resourceLinks);
        }

        // Update nodes
        this.nodeElements = this.nodeElementsG
            .selectAll(".resource-group")
            .data(this.states, n => n.id);

        // Remove excess nodes:
        this.nodeElements
            .exit()
            .transition()
            .attr("opacity", 0)
            .remove();

        // Resource node
        var newNodes = this.nodeElements
            .enter().append("g")
            .attr("class", "resource-group")
            .attr("opacity", 0)
            .attr("resource-name", n => n.id)
            .call(this.dragDrop);

        var newNodesContainer = newNodes
            .append("g")
            .attr("class", "resource-scale")
            .on('click', this.selectNode)
            .on('contextmenu', this.nodeContextMenu);

        // Outline
        newNodesContainer
            .append("circle")
            .attr("r", this.circleSize+2)
            .attr("class", "state-node-border")
            .attr("stroke", "white")
            .attr("stroke-width", "2");

        newNodesContainer
            .append("circle")
            .attr("r", this.circleSize)
            .attr("fill", n => n.color ?? "#999")
            .attr("class", n => {
                return "";
            });

        var resourceNameGroup = newNodesContainer
            .append("g")
            .attr("transform", `translate(${this.circleSize + 5},${this.circleSize/2-5})`)
            .attr("class", "resource-name");
        resourceNameGroup
            .append("text")
            .text(n => n.name)
            .attr("fill", "#999");
        resourceNameGroup
            .append("title")
            .text(n => n.name);

        newNodes.transition()
            .attr("opacity", 1);

        this.nodeElements = newNodes.merge(this.nodeElements);

        this.nodeElementsG
            .selectAll(".resource-group")
            .select(".resource-endpoint")
            .select("title")
            .text(n => n.endpointText);

        // Update links
        this.linkElements = this.linkElementsG
            .selectAll("line")
            .data(this.links, (d) => { return d.id; });

        this.linkElements
            .exit()
            .transition()
            .attr("opacity", 0)
            .remove();

        var newLinks = this.linkElements
            .enter().append("line")
            .attr("opacity", 0)
            .attr("marker-end", "url(#arrow-normal)")
            .attr("class", "resource-link");

        newLinks.transition()
            .attr("opacity", 1);

        this.linkElements = newLinks.merge(this.linkElements);

        this.simulation
            .nodes(this.states)
            .on('tick', this.onTick);

        this.simulation.force("link").links(this.links);
        if (hasStructureChanged) {
            this.simulation.alpha(1).restart();
        }
        else {
            this.simulation.restart();
        }
   }

    onTick = () => {
        this.nodeElements.attr("transform", function (d) { return "translate(" + d.x + "," + d.y + ")"; });
        this.linkElements
            .attr('x1', function (link) { return link.source.x })
            .attr('y1', function (link) { return link.source.y })
            .attr('x2', function (link) { return link.target.x })
            .attr('y2', function (link) { return link.target.y });
    }

    nodeContextMenu = async (event) => {
        var data = event.target.__data__;

        // Prevent default browser context menu.
        event.preventDefault();

        this.openContextMenu = true;

        try {
            // Wait for method completion. It completes when the context menu is closed.
            await this.resourcesInterop.invokeMethodAsync('StateContextMenu', data.id, window.innerWidth, window.innerHeight, event.clientX, event.clientY);
        } finally {
            this.openContextMenu = false;

            // Unselect the node when the context menu is closed to reset mouseover state.
            //this.updateNodeHighlights(null);
        }
    };

    selectNode = (event) => {
        var data = event.target.__data__;

        // Always send the clicked on resource to the server. It will clear the selection if the same resource is clicked again.
        this.resourcesInterop.invokeMethodAsync('SelectState', data.id);

        // Unscale the previous selected node.
        if (this.selectedNode) {
            changeScale(this, this.selectedNode.id, 1);
        }

        // Scale selected node if it is not the same as the previous selected node.
        var clearSelection = this.nodeEquals(data, this.selectedNode);
        if (!clearSelection) {
            changeScale(this, data.id, 1.2);
        }

        this.selectedNode = data;

        function changeScale(self, id, scale) {
            let match = self.nodeElementsG
                .selectAll(".node-group")
                .filter(function (d) {
                    return d.id == id;
                });

            match
                .select(".node-scale")
                .transition()
                .duration(300)
                .style("transform", `scale(${scale})`)
                .on("end", s => {
                    match.select(".node-scale").style("transform", null);
                    //self.updateNodeHighlights(null);
                });
        }
    }


    nodeEquals(resource1, resource2) {
        if (!resource1 || !resource2) {
            return false;
        }
        return resource1.name === resource2.name;
    }
};
