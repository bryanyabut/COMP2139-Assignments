$(document).on('click', '#searchButton', function () {
    const query = $('#searchQuery').val();
    const categoryId = $('#searchCategory').val();

    $.ajax({
        url: '/OrderManagement/Product/Search',
        type: 'GET',
        data: { query, categoryId },
        beforeSend: function () {
            $('#productList').html('<div class="spinner-border"></div>');
        },
        success: function (data) {
            $('#productList').html(data);
        },
        error: function () {
            alert('Error fetching products.');
        }
    });
});