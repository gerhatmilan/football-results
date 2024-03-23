window.getClientTimeAsUTC = function () {
    var localTime = new Date();
    return localTime.toISOString();
};

window.getClientDate = function () {
    const currentDate = new Date();

    // Get the individual components of the date
    const year = currentDate.getFullYear();
    // JavaScript months are zero-based, so we add 1 to get the correct month
    const month = String(currentDate.getMonth() + 1).padStart(2, '0');
    const day = String(currentDate.getDate()).padStart(2, '0');

    // Format the date string as "YYYY-MM-DD"
    const formattedDateString = `${year}-${month}-${day}`;

    return formattedDateString;
}