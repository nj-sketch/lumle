$(function ($) {

    $("#divCalendar").fullCalendar({
        events: {

            url: rootDir + "calendar/events",
            type: "GET",
            error: function () {

            }
        },
        header: {
            left: 'prev,next today',
            center: "title",
            right: 'month,listWeek,listDay'
        },
        views: {
            month :{buttonText: 'Month'},
            listDay: { buttonText: 'List Day' },
            listWeek: { buttonText: 'List Week' }
        },
        eventLimit: true, 
        editable: false,
        eventAfterAllRender: function (view) {
            //Make ajax call to find holidays in range.

            $.ajax({
                url: rootDir + "calendar/holidays",
                type: "GET",
                error: function () {
                  
                },
                success: function (data) {
                    var result = data;
                    if (result.length > 0) {
                        for (var i = 0; i < result.length; i++) {
                            var holiday = moment(result[i].date, 'YYYY-MM-DD');
                            var holidays = [holiday];
                            var holidayMoment;
                            for (var j = 0; j < holidays.length; j++) {
                                holidayMoment = holidays[j];
                                $("td[data-date=" + holidayMoment.format('YYYY-MM-DD') + "]").addClass('holiday');
                                $("td[data-date=" + holidayMoment.format('YYYY-MM-DD') + "]")
                                    .html("<p style='padding:5px;color: #fff; font-size:12px;'>" +
                                        result[i].title +
                                        "</p>");
                            }
                        }
                    }
                }
            });
        }
    });
});

