$(() => {
    ("#like-button").on('click', function () {
        console.log("foo");
        const id = ('image-id').val();
        $.post('/home/likeimage', { id }, function () {
            getLikesForImage();
            ("#like-button").prop('disabled', true);
        });
    });
    function getLikesForImage() {
        const id = ('image-id').val();
        $.get('/home/getlikesforimage', { id }, function (likes) {
            ("#likes-count").text(likes);
        });
    }
});

