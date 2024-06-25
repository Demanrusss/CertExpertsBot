function toUpperCaseFirstLetter(element) {
    if (element !== '') {
        element.value = element.value[0].toUpperCase() + element.value.slice(1);
    }
};

function toRightSymbols(element) {
    if (element !== '') {
        element.value = element.value.replaceAll('см?', 'см3');
    }
};

function trim(element) {
    element.value = element.value.trim();
};

function noDummiesEntrance(element) {
    trim(element);
    toUpperCaseFirstLetter(element);
    toRightSymbols(element);
}