
var tabs = [
    {
        id: 0,
        text: "WIP",
        icon: "card",
        content: "WIP tab content"
    },
    {
        id: 1,
        text: "SHIPPED",
        icon: "box",
        content: "SHIPPED tab content"
    },
    {
        id: 2,
        text: "CANCELLED",
        icon: "clear",
        content: "CANCELLED tab content"
    }
];
$(function () { 
    $("#popover2").dxPopover({
        target: "#link2",
        showEvent: "mouseenter",
        hideEvent: "mouseleave",
        position: "right",
        closeOnBackButton: false,
        closeOnOutsideClick: false,
        width: 300,
        showTitle: true,
        title: "Details:"
    });


    //TimeOut
    var timeOut = null,
        updateTasks = [];
    var timerCallback = function () {
        $.each(updateTasks, function (index, task) {
            task.deferred.resolve();
        });
        updateTasks = [];
        timeOut = null;
    }; 
//TABS
$("#tabs > .tabs-container").dxTabs({
    dataSource: tabs,
    selectedIndex: 0,
    onItemClick: function (e) {
        var id = tabs[e.itemData.id].id;
        if (id === 0) {

            grid;
            $(".idTabs").text(tabs[id].content);
            $(".gridWip").css('display', 'inline');
        } else if (id === 1) {
            $(".gridWip").css('display', 'none');
            $(".idTabs").text(tabs[id].content);
        } else {
            $(".gridWip").css('display', 'none');
            $(".idTabs").text(tabs[id].content);
        }


    }
    }).dxTabs("instance");
});