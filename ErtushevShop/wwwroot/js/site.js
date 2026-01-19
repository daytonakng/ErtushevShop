
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

$(document).ready(function () {
    $('#btn-newProduct').click(function () {
        const id = parseInt($('#newid').val());
        const brand = $('#newbrand').val();
        const model = $('#newmodel').val();
        const description = $('#newdescription').val();
        const category = $('#newcategory').val();
        const price = parseInt($('#newprice').val());
        const shutterspeed = $('#newshutterspeed').val();
        const aperture = $('#newaperture').val();
        const photores = $('#newphotores').val();
        const isorange = $('#newisorange').val();
        const depth = parseInt($('#newdepth').val());
        const duration = parseInt($('#newduration').val());
        const speed = parseInt($('#newspeed').val());
        const height = parseInt($('#newheight').val());
        const sensorsize = $('#newsensorsize').val();
        const viewangle = parseInt($('#newviewangle').val());
        const fixation = $('#newfixation').val();
        const appmanage = $('#newappmanage').val();

        if ((Math.floor(id / 100000) == 4 && category == 'Фотоаппарат') ||
            (Math.floor(id / 100000) == 7 && category == 'Экшн-камера') ||
            (Math.floor(id / 100000) == 9 && category == 'Квадрокоптер')) {

            $.post('/Home/NewProduct', {
                id: id, brand: brand, model: model, description: description, category: category, price: price,
                shutterspeed: shutterspeed, aperture: aperture, photores: photores, isorange: isorange, depth: depth,
                duration: duration, speed: speed, height: height, sensorsize: sensorsize, viewangle: viewangle,
                fixation: fixation, appmanage: appmanage
            }, function (data) {
                if (data.success) {
                    $('#message').removeClass('alert-danger').addClass('alert-success').text(data.message).show();
                    setTimeout(2000);
                    window.location.href = '/Home/Details/' + id;
                } else {
                    $('#message').removeClass('alert-success').addClass('alert-danger').text(data.message).show();
                }
            });
        }
        else {
            showToast();
        }
    });
});

function showToast() {
    const toast = new bootstrap.Toast(document.getElementById('wrongProductToast'));
    toast.show();
};

function showAddToCartToast() {
    const toast = new bootstrap.Toast(document.getElementById('addToCartToast'));
    toast.show();
};

function showEditProfile() {
    const toast = new bootstrap.Toast(document.getElementById('editProfileToast'));
    toast.show();
};

function showSaveProduct() {
    const toast = new bootstrap.Toast(document.getElementById('saveProductToast'));
    toast.show();
};

function showRemoveProduct() {
    const toast = new bootstrap.Toast(document.getElementById('removeProductToast'));
    toast.show();
};

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
};

function saveProduct(button) {
    const form = button.closest('form');

    form.addEventListener('submit', function (e) {
        e.preventDefault();
    }, { once: true });

    const data = new FormData(form);

    fetch('/Home/SaveProduct', {
        method: 'POST',
        body: data
    })
        .then(response => {
            console.log('Данные товара успешно изменены!');
            showSaveProduct();
        });
};

function editProduct(id) {
    window.location.href = '/Home/EditProduct/' + id;
};

function removeProduct(id) {
    window.location.href = '/Home/RemoveProduct/' + id;
};

function clearFields() {
    document.querySelectorAll('input[type="text"], textarea')
        .forEach(input => input.value = '');
};


function updateContent() {
    const select = document.getElementById('newcategory');
    const variableFields = document.getElementById('variableFields');
    const selectedValue = select.value;

    switch (selectedValue) {
        case "Фотоаппарат":
            variableFields.innerHTML = `
               <div class="mb-3">
                    <label for="SensorSize" class="form-label">Размер сенсора</label>
                    <input id="newsensorsize" type="text" class="form-control" name="sensorsize" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="ShutterSpeed" class="form-label">Выдержка</label>
                    <input id="newshutterspeed" type="text" class="form-control" name="shutterspeed" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="Aperture" class="form-label">Диафрагма</label>
                    <input id="newaperture" type="text" class="form-control" name="aperture" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="IsoRange" class="form-label">Диапазон ISO</label>
                    <input id="newisorange" type="text" class="form-control" name="isorange" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="PhotoRes" class="form-label">Разрешение фото</label>
                    <input id="newphotores" type="text" class="form-control" name="photores" placeholder="">
                </div>
            `;
            break;
        case "Экшн-камера":
            variableFields.innerHTML = `
                <div class="mb-3">
                    <label for="ViewAngle" class="form-label">Угол обзора</label>
                    <input id="newviewangle" type="text" class="form-control" name="viewangle" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="Depth" class="form-label">Глубина погружения</label>
                    <input id="newdepth" type="text" class="form-control" name="depth" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="Fixation" class="form-label">Крепление</label>
                    <input id="newfixation" type="text" class="form-control" name="fixation" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="AppManage" class="form-label">Управление через приложение</label>
                    <input id="newappmanage" type="text" class="form-control" name="appmanage" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="IsoRange" class="form-label">Диапазон ISO</label>
                    <input id="newisorange" type="text" class="form-control" name="isorange" placeholder="">
                </div>
            `;
            break;
        case "Квадрокоптер":
            variableFields.innerHTML = `
                <div class="mb-3">
                    <label for="Speed" class="form-label">Скорость</label>
                    <input id="newspeed" type="text" class="form-control" name="speed" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="Height" class="form-label">Максимальная высота</label>
                    <input id="newheight" type="text" class="form-control" name="height" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="Duration" class="form-label">Время полета</label>
                    <input id="newduration" type="text" class="form-control" name="duration" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="AppManage" class="form-label">Управление через приложение</label>
                    <input id="newappmanage" type="text" class="form-control" name="appmanage" placeholder="">
                </div>
                <div class="mb-3">
                    <label for="PhotoRes" class="form-label">Разрешение фото</label>
                    <input id="newphotores" type="text" class="form-control" name="photores" placeholder="">
                </div>
            `;
            break;
    }
}
