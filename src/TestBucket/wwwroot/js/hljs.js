
window.renderHljsForLanguages = async (cssSelector, languages) => {

    const hljsInstalled = typeof hljs !== 'undefined';

    if (!hljsInstalled) {
        return;
    }
    console.log("renderHljsForLanguages", cssSelector);

    const collection = document.querySelectorAll(cssSelector + " code");
    for (let codeBlock of collection) {
        let processed = false;

        console.log("codeBlock", codeBlock);

        for (let language of languages) { 
            if (codeBlock.classList.contains(language)) {
                try {
                    console.log("hljs.highlightElement", codeBlock);

                    hljs.highlightElement(codeBlock);
                } catch (error) {
                }
                processed = true;
            }
            if (processed) {
                break;
            }
        }
    }
} 