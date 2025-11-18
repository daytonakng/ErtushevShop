
function showOrderForm(id) {
    elem = document.getElementById(id);
    state = elem.style.display;
    if (state == 'none') {
        elem.style.display = 'block';
        document.getElementById('orderBtn').innerText = 'Отменить оформление';
        document.getElementById('orderBtn').style.backgroundColor = "#bb2d3b";
    }
    else {
        elem.style.display = 'none';
        document.getElementById('orderBtn').innerText = 'Оформить заказ';
        document.getElementById('orderBtn').style.backgroundColor = "#157347";
    }
};

document.addEventListener("DOMContentLoaded", function (event) {
    if (document.getElementById("order-block") != null) {
        document.getElementById("order-block").style.display = 'none';
    }
});