$(() => {

    $("#like").on('click', function () {
        
        like(true);
    })

    $("#dislike").on('click', function () {

        like(false);
    })

    function like(likes) {

        $.post("/home/addlike", { jokeId: $(".row").data('id'), likes }, function (like) {

            $("#l").text(like.likes);
            $("#d").text(like.dislikes)
        })
    }


})