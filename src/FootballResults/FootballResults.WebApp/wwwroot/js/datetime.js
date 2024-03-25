window.getClientDate = function (dotnetHelper) {
    const currentDate = new Date();

    const year = currentDate.getFullYear();
    const month = String(currentDate.getMonth() + 1).padStart(2, '0');
    const day = String(currentDate.getDate()).padStart(2, '0');
    const hour = String(currentDate.getHours()).padStart(2, '0');
    const minute = String(currentDate.getMinutes()).padStart(2, '0');

    const formattedDateString = `${year}-${month}-${day} ${hour}:${minute}`;

    dotnetHelper.invokeMethodAsync('UpdateClientDate', formattedDateString);
}