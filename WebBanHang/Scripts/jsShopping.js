$(document).ready(function () {
    ShowCount();
    $('body').on('click', '.btnAddToCart', function (e) {
        e.preventDefault();
        var id = $(this).data('id');
        var quatity = 1;
        var tQuantity = $('#quantity_value').text();
        if (tQuantity != '') {
            quatity = parseInt(tQuantity);
        }

        //alert(id + " " + quatity);
        $.ajax({
            url: '/shoppingcart/addtocart',
            type: 'POST',
            data: { id: id, quantity: quatity },
            success: function (rs) {
                if (rs.Success) {
                    $('#checkout_items, .cart-quantity').text(rs.Count);
                    toastr.success(rs.msg);
                }
            }
        });
    });

    $('body').on('click', '.btnUpdate', function (e) {
        e.preventDefault();
        var id = $(this).data("id");
        var quantity = $('#Quantity_' + id).val();
        Update(id, quantity);
    });

    $('body').on('click', '.btnDelete', function (e) {
        e.preventDefault();
        console.log("Nút Xóa được click!");
        var id = $(this).data('id');
        var conf = confirm("Bạn có chắc muốn xóa sản phẩm này khỏi giỏ hàng?");
        if (conf == true) {
            $.ajax({
                url: '/shoppingcart/Delete',
                type: 'POST',
                data: { id: id },
                success: function (rs) {
                    if (rs.Success) {
                        $('#checkout_items, .cart-quantity').text(rs.Count);
                        $('#trow_' + id).remove();
                        LoadCart();
                        toastr.success(rs.msg);
                    }
                }
            });
        }
    });

    $('body').on('click', '.btnDeleteAll', function (e) {
        e.preventDefault();
        var conf = confirm("Bạn có chắc muốn xóa tất cả sản phẩm khỏi giỏ hàng?");
        if (conf == true) {
            DeleteAll();
        }
    });

    // Xử lý tăng/giảm số lượng
    $('body').on('click', '.btnMinus', function() {
        var id = $(this).data('id');
        var input = $('#Quantity_' + id);
        var quantity = parseInt(input.val());
        if (quantity > 1) {
            input.val(quantity - 1);
            Update(id, quantity - 1);
        }
    });

    $('body').on('click', '.btnPlus', function() {
        var id = $(this).data('id');
        var input = $('#Quantity_' + id);
        var quantity = parseInt(input.val());
        input.val(quantity + 1);
        Update(id, quantity + 1);
    });

    // Xử lý khi người dùng nhập số lượng trực tiếp
    $('body').on('change', 'input[id^="Quantity_"]', function() {
        var id = $(this).attr('id').split('_')[1];
        var quantity = parseInt($(this).val());
        if (quantity > 0) {
            Update(id, quantity);
        } else {
            $(this).val(1);
            Update(id, 1);
        }
    });
});

function ShowCount() {
    $.ajax({
        url: '/shoppingcart/ShowCount',
        type: 'GET',
        success: function (rs) {
            $('#checkout_items, .cart-quantity').text(rs.Count);
        }
    });
}

function LoadCart() {
    $.ajax({
        url: '/shoppingcart/Partial_Item_Cart',
        type: 'GET',
        success: function (rs) {
            $('#load_data').html(rs);
        }
    });
}

function DeleteAll() {
    $.ajax({
        url: '/shoppingcart/DeleteAll',
        type: 'POST',
        success: function (rs) {
            if (rs.Success) {
                $('#checkout_items, .cart-quantity').text('0');
                LoadCart();
                toastr.success('Đã xóa tất cả sản phẩm khỏi giỏ hàng');
            }
        }
    });
}

function Update(id, quantity) {
    $.ajax({
        url: '/shoppingcart/Update',
        type: 'POST',
        data: { id: id, quantity: quantity },
        success: function (rs) {
            if (rs.Success) {
                LoadCart();
                $('#checkout_items, .cart-quantity').text(rs.Count);
                toastr.success(rs.msg);
            } else {
                toastr.error(rs.msg);
            }
        },
        error: function() {
            toastr.error('Có lỗi xảy ra khi cập nhật số lượng');
        }
    });
}
