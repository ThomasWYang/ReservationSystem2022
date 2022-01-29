
(function ($) {

    var defaults = {
        timeslotHeight: 40,
        timeslotWidth: 90,
        sittingDate: '2019-01-01',
        sittingName: 'Breakfast',
        currentReservationId: 1,
        timeslots: [{ id: 1, startTime: "8:00:00", endTime: "9:00:00" }, { id: 2, startTime: "9:00:00", endTime: "10:00:00" }, { id: 3, startTime: "10:00:00", endTime: "11:00:00" }],
        tables: [{ id: 1, name: "A1" }, { id: 2, name: "A2" }],
        sittingUnits: [
            { id: 1, timeslotId: 1, tableId: 1, reservationId: null },
            { id: 2, timeslotId: 1, tableId: 2, reservationId: null },
            { id: 3, timeslotId: 2, tableId: 1, reservationId: 1 },
            { id: 4, timeslotId: 2, tableId: 2, reservationId: 2 },
            { id: 5, timeslotId: 3, tableId: 1, reservationId: null},
            { id: 6, timeslotId: 3, tableId: 2, reservationId: null }],
        clickEnabled: true,
    }

    $.fn.scheduler = function (options) {

        //Merge the contents of options into the defaults.

        var settings = $.extend({}, defaults, options);

        return this.each(function () {

            render.calender($(this), settings);
            setSize(settings.timeslotHeight, settings.timeslotWidth, settings.responsiveWidth);
            if (settings.clickEnabled==true) { attachEventListeners(settings) };


        });



    }

    //render scheduler

    var render = {

        calender: function (container, settings) {

            var header = render.header(settings);
            var body = render.body(settings);

            container.append(header);
            container.append(body);

        },

        header: function (settings) {
            var header = $("<div></div>").addClass("header");
            var sittingDate = $("<div></div>").addClass("sittingdate").text(settings.sittingDate);
            var sittingName = $("<div></div>").addClass("sittingname").text(settings.sittingName);

            header.append(sittingDate);
            header.append(sittingName);

            return header;
        },

        body: function (settings) {
            var body = $("<div></div>").addClass("body");

            var rowHeader = render.rowHeader(settings);
            var timeslots = render.timeslots(settings);

            body.append(rowHeader);
            body.append(timeslots);


            return body;
        },


        rowHeader: function (settings) {

            var rowHeaderContainer = $("<div></div>").addClass("row-header-container");
            rowHeaderContainer.append("<div class='hour-header hour'>Schedule</div>");
            var i;
            for (i = 0; i < settings.tables.length; i++) {
                var header = $("<div></div>").addClass("row-header cell").text(settings.tables[i].name);
                rowHeaderContainer.append(header);
            }

            return rowHeaderContainer;
        },

        timeslots: function (settings) {

            var columnHeader = $("<div></div>").addClass("column-header");
            var rowContainer = $("<div></div>").addClass("row-container");
            var rsvnContainer = $("<div></div>").addClass("rsvn-container");



            for (i = 0; i < settings.timeslots.length; i++) {
                var hour = $("<div></div>").addClass("responsiveHour").text(settings.timeslots[i].startTime + " - " + settings.timeslots[i].endTime);
                columnHeader.append(hour);
            }
            rowContainer.append(columnHeader);
            rowContainer.append(rsvnContainer);

            for (var i = 0; i < settings.tables.length; i++) {
                var row = $("<div></div>").addClass("row");
                rowContainer.append(row);


                for (var j = 0; j < settings.timeslots.length; j++) {

                    var currentSittingUnit = settings.sittingUnits.find(x => x.tableId == settings.tables[i].id && x.timeslotId == settings.timeslots[j].id);
                    var timeslot = $("<div></div>").addClass("timeslot responsiveCell");
                    timeslot.attr("id", currentSittingUnit.id);
                    if (currentSittingUnit.reservationId != null) {
                        if (currentSittingUnit.reservationId != settings.currentReservationId) { timeslot.addClass("occupied"); }
                        else { timeslot.addClass("booked"); };
                    }


                    row.append(timeslot);
                }
            }

            return rowContainer;
        },
    }


    //Attach event listeners for the scheduler
    var attachEventListeners = function (settings) {
        $(".timeslot").on("click", { 'settings': settings }, listeners.timeslot);
    }

    //Event listner functions to attach
    var listeners = {

        timeslot: function (e) {
            if (!$(e.target).hasClass("occupied")) {
                if (!$(e.target).hasClass("booked")) { $(e.target).addClass("booked") }
                else { $(e.target).removeClass("booked") }
            }

        }

    }


    // Sets the size of the scheduler timeslots
    var setSize = function (fixedHeight, fixedWidth, responsiveWidth) {
        $(".hour").css({
            minWidth: fixedWidth,
            maxWidth: fixedWidth
        });
        $(".cell").css({
            minHeight: fixedHeight,
            minWidth: fixedWidth,
            maxHeight: fixedHeight,
            maxWidth: fixedWidth
        });
        $('.row-container').css({
            width: '100%'
        })
        $(".responsiveHour").css({
            width: responsiveWidth
        });
        $(".responsiveCell").css({
            minHeight: fixedHeight,
            maxHeight: fixedHeight,
            width: responsiveWidth
        });
    }

}(jQuery));