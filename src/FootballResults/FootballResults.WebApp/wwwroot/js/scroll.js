window.scrollToBottom = function (elementReference, lastScrollPos) {
    const currentScrollPos = elementReference.scrollTop ?? 0;

    if (currentScrollPos > lastScrollPos)
        elementReference.scrollTop = elementReference.scrollHeight;

    return parseInt(currentScrollPos);
}