$(() => {

    const id = $('#image-id').val();

    setInterval(() => {
        $.get('/home/getlikesforimage', { id }, function (likes) {
            $("#likes-count").text(likes);
        });
    }, 1000);

    $("#like-button").on('click', function () {
        console.log("foo");
      
        $.post('/home/likeimage', { id }, function () {
            $("#like-button").prop('disabled', true);
        });
    });
});

