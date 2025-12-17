
function showOrderForm(id) {
    elem = document.getElementById(id);
    btn = document.getElementById('orderBtn');
    clearBtn = document.getElementById('clearCartBtn');
    state = elem.style.display;
    if (state == 'none') {
        elem.style.display = 'block';
        btn.innerText = 'Отменить оформление';
        btn.style.backgroundColor = "#bb2d3b";
        clearBtn.style.display = 'none';
    }
    else {
        elem.style.display = 'none';
        btn.innerText = 'Оформить заказ';
        btn.style.backgroundColor = "#157347";
        clearBtn.style.display = 'block';

        form = document.getElementById("orderForm");

        btn.addEventListener('click', () => {
            form.reset();
        });
    }
};

document.addEventListener("DOMContentLoaded", function (event) {
    if (document.getElementById("order-block") != null) {
        document.getElementById("order-block").style.display = 'none';
    }
});

function handleAddToCart(button) {
    const form = button.closest('form');

    form.addEventListener('submit', function (e) {
        e.preventDefault();
    }, { once: true });

    const data = new FormData(form);

    fetch('/Home/AddToCart', {
        method: 'POST',
        body: data
    })
        .then(response => {
            if (response.ok) {
                console.log('Товар добавлен в корзину!');
                showAddToCartToast();
            } else {
                console.error('Ошибка при добавлении в корзину');
            }
        });
}

function showAddToCartToast() {
    const toast = new bootstrap.Toast(document.getElementById('addToCartToast'));
    toast.show();
}

function addOrder() {
    document.getElementById('orderForm').addEventListener('submit', function (e) {
        e.preventDefault();

        const form = e.target;
        const data = new FormData(form);

        fetch('/Home/Create', {
            method: 'POST',
            body: data
        })
            .then(response => {
                if (response.ok) {
                    console.log(`Заказ оформлен!`);
                }
            });
    });
}

$(document).ready(function () {
    $('#show-register').click(function (e) {
        e.preventDefault();
        $('#login-form').hide();
        $('#register-form').show();
        $('#message').hide();
    });

    $('#show-login').click(function (e) {
        e.preventDefault();
        $('#register-form').hide();
        $('#login-form').show();
        $('#message').hide();
    });

    $('#btn-login').click(function () {
        const username = $('#login-username').val();
        const password = $('#login-password').val();

        $.ajax({
            url: '/Home/Login',
            type: 'POST',
            data: { username: username, password: password },
            success: function (response) {
                if (response.success) {
                    window.location.href = response.redirectUrl;
                } else {
                    $('#message')
                        .removeClass('alert-success')
                        .addClass('alert-danger')
                        .text(response.message)
                        .show();
                }
            },
            error: function () {
                $('#message')
                    .removeClass('alert-success')
                    .addClass('alert-danger')
                    .text('Произошла ошибка.')
                    .show();
            }
        });
    });

    $('#btn-register').click(function () {
        const username = $('#register-username').val();
        const password = $('#register-password').val();

        $.post('/Home/Register', { username: username, password: password }, function (data) {
            if (data.success) {
                $('#message').removeClass('alert-danger').addClass('alert-success').text(data.message).show();
                setTimeout(function () {
                    $('#register-form').hide();
                    $('#login-form').show();
                }, 2000);
            } else {
                $('#message').removeClass('alert-success').addClass('alert-danger').text(data.message).show();
            }
        });
    });
});

//$(document).ready(function () {
//    $('#btn-saveprofile').click(function () {
//        const lastName = $('#lastName').val();
//        const firstName = $('#firstName').val();
//        const middleName = $('#middleName').val();
//        const phone = $('#phone').val();
//        const email = $('#email').val();

//        $.post('/Home/EditProfile', { lastName: lastName, firstName: firstName, middleName: middleName, phone: phone, email: email }, function (data) {
//            if (data.success) {
//                $('#message').removeClass('alert-danger').addClass('alert-success').text(data.message).show();
//                setTimeout(function () {
//                }, 2000);
//            } else {
//                $('#message').removeClass('alert-success').addClass('alert-danger').text(data.message).show();
//            }
//        });
//    })
//})

function showEditProfile() {
    const toast = new bootstrap.Toast(document.getElementById('editProfileToast'));
    toast.show();
}

function editProfile(button) {
    const form = button.closest('form');

    form.addEventListener('submit', function (e) {
        e.preventDefault();
    }, { once: true });

    const data = new FormData(form);

    fetch('/Home/EditProfile', {
        method: 'POST',
        body: data
    })
        .then(response => {
            if (response.ok) {
                console.log('Данные пользователя успешно изменены!');
                showEditProfile();
            } else {
                console.error('Ошибка изменения данных пользователя!');
            }
        });
}

