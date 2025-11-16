const cartState = new Set();
const favoriteState = new Set();

function toggleCart(button) {
    const productId = button.getAttribute('data-product-id');
    const icon = button.querySelector('i');

    if (cartState.has(productId)) {
        cartState.delete(productId);
        icon.className = 'bi bi-basket';
        button.classList.remove('btn-success');
        button.classList.add('btn-outline-secondary');
        //alert(`Товар ${productId} удален из корзины`);
    } else {
        cartState.add(productId);
        icon.className = 'bi bi-basket-fill';
        button.classList.remove('btn-outline-secondary');
        button.classList.add('btn-success');
        //alert(`Товар ${productId} добавлен в корзину`);
    }
    localStorage.setItem('cart', JSON.stringify([...cartState]));
}

function toggleFavorite(button) {
    const productId = button.getAttribute('data-product-id');
    const icon = button.querySelector('i');

    if (favoriteState.has(productId)) {
        favoriteState.delete(productId);
        icon.className = 'bi bi-heart';
        button.classList.remove('btn-danger');
        button.classList.add('btn-outline-danger');
        //alert(`Товар ${productId} удален из избранного`);
    } else {
        favoriteState.add(productId);
        icon.className = 'bi bi-heart-fill';
        button.classList.remove('btn-outline-danger');
        button.classList.add('btn-danger');
        //alert(`Товар ${productId} добавлен в избранное`);
    }

    localStorage.setItem('favorites', JSON.stringify([...favoriteState]));
}

document.addEventListener('DOMContentLoaded', () => {
    const savedCart = JSON.parse(localStorage.getItem('cart')) || [];
    const savedFavorites = JSON.parse(localStorage.getItem('favorites')) || [];

    savedCart.forEach(id => cartState.add(id));
    savedFavorites.forEach(id => favoriteState.add(id));

    document.querySelectorAll('[data-action="cart"]').forEach(button => {
        const id = button.getAttribute('data-product-id');
        if (cartState.has(id)) {
            button.querySelector('i').className = 'bi bi-basket-fill';
            button.classList.replace('btn-outline-secondary', 'btn-success');
        }
    });

    document.querySelectorAll('[data-action="favorite"]').forEach(button => {
        const id = button.getAttribute('data-product-id');
        if (favoriteState.has(id)) {
            button.querySelector('i').className = 'bi bi-heart-fill';
            button.classList.replace('btn-outline-danger', 'btn-danger');
        }
    });
    
});