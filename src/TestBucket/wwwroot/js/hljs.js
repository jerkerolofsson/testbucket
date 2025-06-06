
window.renderHljsForLanguages = async (cssSelector, languages, code) => {

    const hljsInstalled = typeof hljs !== 'undefined';

    if (!hljsInstalled) {
        return;
    }
    const collection = document.querySelectorAll(cssSelector + " code");
    console.log("renderHljsForLanguages", cssSelector, collection);
    for (let codeBlock of collection) {
        let processed = false;

        for (let language of languages) { 

            console.log("renderHljsForLanguages.language=" + language, code);
            console.log("code = " + code);

            if (codeBlock.classList.contains(language)) {
                try {
                    
                    if (hljs.getLanguage(language)) {
                        const res = hljs.highlight(code, { language });
                        console.log(res);
                        codeBlock.innerHTML = res.value;
                    } else {
                        const res = hljs.highlightAuto(code, { language });
                        console.log(res);
                        codeBlock.innerHTML = res.value;
                    }
                } catch (error) {
                }
                processed = true;
            } else {
                console.log("renderHljsForLanguages.language doesnt match", codeBlock.classList);
                codeBlock.innerHTML = code;
            }
            if (processed) {
                break;
            }
        }
    }
} 
