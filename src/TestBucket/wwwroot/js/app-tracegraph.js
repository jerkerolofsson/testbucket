import './d3.v7.min.js'

let resourceGraph = null;


export function initializeNodes(resourcesInterop) {
    resourceGraph = new ResourceGraph(resourcesInterop);
    resourceGraph.resize();

    const observer = new ResizeObserver(function () {
        resourceGraph.resize();
    });

    for (const child of document.getElementsByClassName('trace-graph')) {
        observer.observe(child);
    }
}

export function updateGraph(resources) {

    console.log("updateGraph", resources);

    if (resourceGraph) {
        try {
            resourceGraph.updateResources(resources);
        } catch (e) {
            console.error(e);
        }
    }
}

export function updateResourcesGraphSelected(resourceName) {
    if (resourceGraph) {
        resourceGraph.switchTo(resourceName);
    }
}

class ResourceGraph {
    constructor(resourcesInterop) {
        this.resources = [];
        this.resourcesInterop = resourcesInterop;
        this.openContextMenu = false;

        this.nodes = [];
        this.links = [];

        this.svg = d3.select('.trace-graph');
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
            .force("x", d3.forceX().strength(0.1))
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
        this.createArrowMarker(defs, "arrow-normal", "arrow-normal", 10, 10, 66);
        this.createArrowMarker(defs, "arrow-highlight", "arrow-highlight", 15, 15, 48);
        this.createArrowMarker(defs, "arrow-highlight-expand", "arrow-highlight-expand", 15, 15, 56);

        var highlightedPattern = defs.append("pattern")
            .attr("id", "highlighted-pattern")
            .attr("patternUnits", "userSpaceOnUse")
            .attr("width", "17.5")
            .attr("height", "17.5")
            .attr("patternTransform", "rotate(45)");

        highlightedPattern
            .append("rect")
            .attr("x", "0")
            .attr("y", "0")
            .attr("width", "17.5")
            .attr("height", "17.5")
            .attr("fill", "var(--fill-color, red)");

        highlightedPattern
            .append("line")
            .attr("x1", "0")
            .attr("y", "0")
            .attr("x2", "0")
            .attr("y2", "17.5")
            .attr("stroke", "var(--neutral-fill-secondary-hover, magenta)")
            .attr("stroke-width", "15");

        this.linkElementsG = this.baseGroup.append("g").attr("class", "links").attr("stroke", "#99F");
        this.nodeElementsG = this.baseGroup.append("g").attr("class", "nodes");

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
            .attr("markerWidth", width)
            .attr("markerHeight", height)
            .attr("orient", "auto")
            .attr("markerUnits", "userSpaceOnUse")
            .attr("class", className)
            .append("path")
            .attr("d", 'M0,-5L10,0L0,5');
    }

    resize() {
        var container = document.querySelector(".trace-graph");
        if (container) {
            var width = container.clientWidth;
            var height = Math.max(container.clientHeight - 50, 0);
            this.svg.attr("viewBox", [-width / 2, -height / 2, width, height]);
        }
    }

    switchTo(resourceName) {
        this.selectedNode = this.nodes.find(node => node.id === resourceName);
        this.updateNodeHighlights(null);
    }

    resourceEqual(r1, r2) {
        if (r1.id !== r2.id) {
            return false;
        }
        if (r1.type !== r2.type) {
            return false;
        }
        if (r1.name !== r2.name) {
            return false;
        }
        if (!this.iconEqual(r1.icon, r2.icon)) {
            return false;
        }
        if (r1.references.length !== r2.references.length) {
            return false;
        }
        for (var i = 0; i < r1.references.length; i++) {
            if (r1.references[i] !== r2.references[i]) {
                return false;
            }
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

    resourcesChanged(existingResource, newResources) {
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

    updateNodes(newResources) {
        const existingNodes = this.nodes || []; // Ensure nodes is initialized
        const updatedNodes = [];

        newResources.forEach(resource => {
            const existingNode = existingNodes.find(node => node.id === resource.id);

            if (existingNode) {
                // Update existing node without replacing it
                updatedNodes.push({
                    ...existingNode,
                    name: resource.name,
                    type: resource.type,
                    icon: createIcon(resource.icon),
                });
            } else {
                // Add new resource
                updatedNodes.push({
                    id: resource.id,
                    name: resource.name,
                    type: resource.type,
                    icon: createIcon(resource.icon),
                });
            }
        });

        this.nodes = updatedNodes;

        function createIcon(resourceIcon) {
            return {
                svg: resourceIcon.svg,
                color: resourceIcon.color,
                tooltip: resourceIcon.tooltip
            };
        }
    }

    updateResources(newResources) {
        // Check if the overall structure of the graph has changed. i.e. nodes or links have been added or removed.
        var hasStructureChanged = this.resourcesChanged(this.resources, newResources);

        this.resources = newResources;

        this.updateNodes(newResources);

        this.links = [];
        for (var i = 0; i < newResources.length; i++) {
            var resource = newResources[i];

            var resourceLinks = resource.references
                .filter((refId) => {
                    return newResources.filter(r => r.id === refId);
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
            .data(this.nodes, n => n.id);

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
            .on('contextmenu', this.nodeContextMenu)
            .on('mouseover', this.hoverNode)
            .on('mouseout', this.unHoverNode);

            // Outline
        newNodesContainer
            .append("circle")
            .attr("r", 35)
            .attr("class", "resource-node")
            .attr("stroke", "white")
            .attr("stroke-width", "4");

        newNodesContainer
            .append("circle")
            .attr("r", 33)
            .attr("fill", n => {
                console.log("fill", n);
                return (n.type == "Requirement" ? "var(--mud-palette-primary-darken)" : "var(--mud-palette-secondary-darken)")
            })
            .attr("class", "resource-node-border");

        // Icon
        newNodesContainer
            .append("g")
            .attr("transform", "scale(2.1) translate(-10,-10)")
            .append("svg")
            .attr("color", n => n.icon.color)
            .attr("width", "20").attr("height", "20")
            .html(n => n.icon.svg);

            //.attr("d", n => n.icon.path)
            //.append("title")
            //.text(n => "HELLO" ?? n.icon.tooltip);

        console.log("newResources", newResources);
        var resourceNameGroup = newNodesContainer
            .append("g")
            .attr("transform", "translate(0,71)")
            .attr("class", "resource-name");
        resourceNameGroup
            .append("text")
            .text(n => trimText(n.name, 30) )
            .attr("fill", "#999");
        resourceNameGroup
            .append("title")
            .text(n => n.name);

        newNodes.transition()
            .attr("opacity", 1);

        this.nodeElements = newNodes.merge(this.nodeElements);

        // Set resource values that change.
        //this.nodeElementsG
        //    .selectAll(".resource-group")
        //    .select(".resource-endpoint")
        //    .select("text")
        //    .text(n => trimText(n.endpointText, 15));
        this.nodeElementsG
            .selectAll(".resource-group")
            .select(".resource-endpoint")
            .select("title")
            .text(n => n.endpointText);
        //this.nodeElementsG
        //    .selectAll(".resource-group")
        //    .select(".resource-status-circle")
        //    .select("title")
        //    .text(n => n.stateIcon.tooltip);
        //this.nodeElementsG
        //    .selectAll(".resource-group")
        //    .select(".resource-status-path")
        //    .attr("d", n => n.icon.path)
        //    .attr("fill", n => n.icon.color)
        //    .select("title")
        //    .text(n => n.icon.tooltip);

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
            .attr("class", "resource-link");

        newLinks.transition()
            .attr("opacity", 1);

        this.linkElements = newLinks.merge(this.linkElements);

        this.simulation
            .nodes(this.nodes)
            .on('tick', this.onTick);

        this.simulation.force("link").links(this.links);
        if (hasStructureChanged) {
            this.simulation.alpha(1).restart();
        }
        else {
            this.simulation.restart();
        }

        function trimText(text, maxLength) {
            if (text && text.length > maxLength) {
                return text.slice(0, maxLength) + "\u2026";
            }
            return text;
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

    getNeighbors(node) {
        return this.links.reduce(function (neighbors, link) {
            if (link.target.id === node.id) {
                neighbors.push(link.source.id);
            } else if (link.source.id === node.id) {
                neighbors.push(link.target.id);
            }
            return neighbors;
        },
            [node.id]);
    }

    isNeighborLink(node, link) {
        return link.target.id === node.id || link.source.id === node.id
    }

    getLinkClass(nodes, selectedNode, link) {
        if (nodes.find(n => this.isNeighborLink(n, link))) {
            if (this.nodeEquals(selectedNode, link.target)) {
                return 'resource-link-highlight-expand';
            }
            return 'resource-link-highlight';
        }
        return 'resource-link';
    }

    nodeContextMenu = async (event) => {
        var data = event.target.__data__;

        // Prevent default browser context menu.
        event.preventDefault();

        this.openContextMenu = true;

        try {
            // Wait for method completion. It completes when the context menu is closed.
            await this.resourcesInterop.invokeMethodAsync('ResourceContextMenu', data.id, window.innerWidth, window.innerHeight, event.clientX, event.clientY);
        } finally {
            this.openContextMenu = false;

            // Unselect the node when the context menu is closed to reset mouseover state.
            this.updateNodeHighlights(null);
        }
    };

    selectNode = (event) => {
        var data = event.target.__data__;

        // Always send the clicked on resource to the server. It will clear the selection if the same resource is clicked again.
        this.resourcesInterop.invokeMethodAsync('SelectResource', data.id);

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
                .selectAll(".resource-group")
                .filter(function (d) {
                    return d.id == id;
                });

            match
                .select(".resource-scale")
                .transition()
                .duration(300)
                .style("transform", `scale(${scale})`)
                .on("end", s => {
                    match.select(".resource-scale").style("transform", null);
                    self.updateNodeHighlights(null);
                });
        }
    }

    hoverNode = (event) => {
        var mouseoverNode = event.target.__data__;

        this.updateNodeHighlights(mouseoverNode);
    }

    unHoverNode = (event) => {
        // Don't unhover the selected node when the context menu is open.
        // This is done to keep the node selected until the context menu is closed.
        if (!this.openContextMenu) {
            this.updateNodeHighlights(null);
        }
    };

    nodeEquals(resource1, resource2) {
        if (!resource1 || !resource2) {
            return false;
        }
        return resource1.id === resource2.id;
    }

    updateNodeHighlights = (mouseoverNode) => {
        var mouseoverNeighbors = mouseoverNode ? this.getNeighbors(mouseoverNode) : [];
        var selectNeighbors = this.selectedNode ? this.getNeighbors(this.selectedNode) : [];
        var neighbors = [...mouseoverNeighbors, ...selectNeighbors];

        // we modify the styles to highlight selected nodes
        this.nodeElements.attr('class', (node) => {
            var classNames = ['resource-group'];
            if (this.nodeEquals(node, mouseoverNode)) {
                classNames.push('resource-group-hover');
            }
            if (this.nodeEquals(node, this.selectedNode)) {
                classNames.push('resource-group-selected');
            }
            if (neighbors.indexOf(node.id) > -1) {
                classNames.push('resource-group-highlight');
            }
            return classNames.join(' ');
        });
        this.linkElements.attr('class', (link) => {
            var nodes = [];
            if (mouseoverNode) {
                nodes.push(mouseoverNode);
            }
            if (this.selectedNode) {
                nodes.push(this.selectedNode);
            }
            return this.getLinkClass(nodes, this.selectedNode, link);
        });
    };
};
