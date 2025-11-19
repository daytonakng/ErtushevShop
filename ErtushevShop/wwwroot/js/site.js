
function showOrderForm(id) {
    elem = document.getElementById(id);
    btn = document.getElementById('orderBtn');
    state = elem.style.display;
    if (state == 'none') {
        elem.style.display = 'block';
        btn.innerText = 'Отменить оформление';
        btn.style.backgroundColor = "#bb2d3b";
    }
    else {
        elem.style.display = 'none';
        btn.innerText = 'Оформить заказ';
        btn.style.backgroundColor = "#157347";

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

function addToCart() {
    document.getElementById('addToCartForm').addEventListener('submit', function (e) {
        e.preventDefault();

        const form = e.target;
        const data = new FormData(form);

        fetch('/Home/AddToCart', {
            method: 'POST',
            body: data
        })
            .then(response => {
                if (response.ok) {
                    console.log(`Товар добавлен в корзину!`);
                }
            });
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

function showAddToCartToast() {
    const toast = new bootstrap.Toast(document.getElementById('addToCartToast'));
    toast.show();
}
