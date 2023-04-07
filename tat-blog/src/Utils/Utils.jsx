export function isEmptyOrSpaces(str) {
  return (
    str === null || (typeof str === "string" && str.match(/^ *$/) !== null)
  );
}

export function getMonthName(monthNumber) {
  const month = [
    "January",
    "February",
    "March",
    "April",
    "May",
    "June",
    "July",
    "August",
    "September",
    "October",
    "November",
    "December",
  ];

  return month[monthNumber - 1];
}
