window.scrollToBottomIfLastScrollDown = function (elementReference, lastScrollPos) {
    const currentScrollPos = elementReference.scrollTop;

    if (parseInt(currentScrollPos) > lastScrollPos)
        elementReference.scrollTop = elementReference.scrollHeight;

    return parseInt(currentScrollPos);
}

window.scrollToBottom = function (elementReference) {
    elementReference.scrollTop = elementReference.scrollHeight;
}