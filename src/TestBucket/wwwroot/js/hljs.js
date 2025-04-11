
window.renderHljsForLanguages = async (cssSelector, languages) => {

    const hljsInstalled = typeof hljs !== 'undefined';

    if (!hljsInstalled) {
        return;
    }
    const collection = document.querySelectorAll(cssSelector + " code");
    console.log("renderHljsForLanguages", cssSelector, collection);
    for (let codeBlock of collection) {
        let processed = false;

        for (let language of languages) { 

            console.log("renderHljsForLanguages.language=" + language);

            if (codeBlock.classList.contains(language)) {
                try {
                    console.log("hljs.highlightElement", codeBlock);

                    hljs.highlightElement(codeBlock);
                } catch (error) {
                }
                processed = true;
            } else {
                console.log("renderHljsForLanguages.language doesnt match", codeBlock.classList);
            }
            if (processed) {
                break;
            }
        }
    }
} 
