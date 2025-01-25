kamaDatepicker('carWashDate', {

    // placeholder text
    placeholder: "",

    // enable 2 digits
    twodigit: true,

    // close calendar after select
    closeAfterSelect: true,

    // nexy / prev buttons
    nextButtonIcon: "بعدی",
    previousButtonIcon: "قبلی ",

    // color of buttons
    buttonsColor: "پیشفرض ",

    // force Farsi digits
    forceFarsiDigits: false,

    // highlight today
    markToday: false,

    // highlight holidays
    markHolidays: false,

    // highlight user selected day
    highlightSelectedDay: false,

    // true or false
    sync: false,

    // display goto today button
    gotoToday: false,

    // the number of years to be selected before this year
    pastYearsCount: 95,

    // the number of years to be selected after this year
    futureYearsCount: 6,

    // auto swaps next/prev buttons
    swapNextPrev: false,

    // define custom holidays here
    holidays: [
        { month: 1, day: 1 },
        { month: 1, day: 2 },
        { month: 1, day: 3 },
        { month: 1, day: 4 },
        { month: 1, day: 12 },
        { month: 1, day: 13 },
        { month: 3, day: 14 },
        { month: 3, day: 15 },
        { month: 11, day: 22 },
        { month: 12, day: 29 }
    ];

    // set disabled holidays
    disableHolidays: false

});