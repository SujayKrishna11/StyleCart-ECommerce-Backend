import { useState } from "react";
import "./App.css";

import {
    addItemToCart,
    getCart,
    getProductVariants,
    removeCartItem,
    updateCartItem,
} from "./api/styleCartApi";

import Header from "./components/Header";
import VariantPicker from "./components/VariantPicker";

import CartPage from "./pages/CartPage";
import CheckoutPage from "./pages/CheckoutPage";
import LoginPage from "./pages/LoginPage";
import OrdersPage from "./pages/OrdersPage";
import ProductsPage from "./pages/ProductsPage";
import RegisterPage from "./pages/RegisterPage";

import type {
    Cart,
    LoginResponse,
    Order,
    Product,
    ProductVariant,
} from "./types/models";

type Page =
    | "products"
    | "login"
    | "register"
    | "cart"
    | "checkout"
    | "orders";

function App() {
    const [currentPage, setCurrentPage] = useState<Page>("products");

    const [token, setToken] = useState(
        localStorage.getItem("styleCartToken") ?? "",
    );

    const [cart, setCart] = useState<Cart | null>(null);

    const [selectedProduct, setSelectedProduct] =
        useState<Product | null>(null);

    const [availableVariants, setAvailableVariants] = useState<
        ProductVariant[]
    >([]);

    const [message, setMessage] = useState("");
    const [error, setError] = useState("");

    const cartItemCount =
        cart?.items.reduce((total, item) => total + item.quantity, 0) ?? 0;

    async function loadCart(activeToken = token) {
        if (!activeToken) {
            return;
        }

        try {
            const cartData = await getCart(activeToken);
            setCart(cartData);
        } catch (requestError) {
            showError(requestError, "Could not load the cart.");
        }
    }

    async function navigateTo(page: Page) {
        setMessage("");
        setError("");

        if (page === "cart" || page === "orders") {
            if (!token) {
                setMessage("Please log in to access this page.");
                setCurrentPage("login");
                return;
            }
        }

        if (page === "cart") {
            await loadCart();
        }

        setCurrentPage(page);
    }

    function handleLogin(response: LoginResponse) {
        localStorage.setItem("styleCartToken", response.token);

        setToken(response.token);
        setMessage(`Welcome, ${response.email}.`);
        setError("");
        setCurrentPage("products");

        loadCart(response.token);
    }

    function handleLogout() {
        localStorage.removeItem("styleCartToken");

        setToken("");
        setCart(null);
        setSelectedProduct(null);
        setAvailableVariants([]);
        setMessage("You have logged out.");
        setError("");
        setCurrentPage("products");
    }

    async function handleAddToCart(product: Product) {
        if (!token) {
            setMessage("Please log in before adding a product to the cart.");
            setCurrentPage("login");
            return;
        }

        try {
            setMessage("");
            setError("");

            const variants = await getProductVariants(product.id, token);

            const activeVariants = variants.filter(
                (variant) => variant.isActive,
            );

            if (activeVariants.length === 0) {
                setError(`${product.name} does not have an active variant yet.`);
                return;
            }

            setSelectedProduct(product);
            setAvailableVariants(activeVariants);
        } catch (requestError) {
            showError(requestError, "Could not load product variants.");
        }
    }

    async function addSelectedVariantToCart(variantId: number) {
        try {
            await addItemToCart(variantId, 1, token);
            await loadCart();

            setSelectedProduct(null);
            setAvailableVariants([]);
            setMessage("Product added to your cart.");
        } catch (requestError) {
            showError(requestError, "Could not add this product to the cart.");
        }
    }

    async function increaseQuantity(cartItemId: number, quantity: number) {
        try {
            await updateCartItem(cartItemId, quantity + 1, token);
            await loadCart();
        } catch (requestError) {
            showError(requestError, "Could not update the quantity.");
        }
    }

    async function decreaseQuantity(cartItemId: number, quantity: number) {
        if (quantity <= 1) {
            return;
        }

        try {
            await updateCartItem(cartItemId, quantity - 1, token);
            await loadCart();
        } catch (requestError) {
            showError(requestError, "Could not update the quantity.");
        }
    }

    async function deleteCartItem(cartItemId: number) {
        try {
            await removeCartItem(cartItemId, token);
            await loadCart();
            setMessage("Item removed from your cart.");
        } catch (requestError) {
            showError(requestError, "Could not remove the item.");
        }
    }

    function goToCheckout() {
        if (!cart || cart.items.length === 0) {
            setError("Your cart is empty.");
            return;
        }

        setMessage("");
        setError("");
        setCurrentPage("checkout");
    }

    function handleOrderPlaced(order: Order) {
        setCart(null);
        setMessage(`Order #${order.id} was placed successfully.`);
        setError("");
        setCurrentPage("orders");
    }

    function closeVariantPicker() {
        setSelectedProduct(null);
        setAvailableVariants([]);
    }

    function showError(requestError: unknown, fallbackMessage: string) {
        if (requestError instanceof Error) {
            setError(requestError.message);
        } else {
            setError(fallbackMessage);
        }
    }

    return (
        <div className= "app" >
        <Header
        currentPage={ currentPage }
    isLoggedIn = { Boolean(token) }
    cartItemCount = { cartItemCount }
    onNavigate = { navigateTo }
    onLogout = { handleLogout }
        />

        <main className="main-content" >
        { message && <p className="success-message" > { message } </p>
}
{ error && <p className="error-message" > { error } </p> }

{
    currentPage === "products" && (
        <ProductsPage onAddToCart={ handleAddToCart } />
        )
}

{
    currentPage === "login" && (
        <LoginPage onLogin={ handleLogin } />
        )
}

{
    currentPage === "register" && (
        <RegisterPage onRegister={ handleLogin } />
        )
}

{
    currentPage === "cart" && (
        <CartPage
            cart={ cart }
    onIncreaseQuantity = { increaseQuantity }
    onDecreaseQuantity = { decreaseQuantity }
    onRemoveItem = { deleteCartItem }
    onCheckout = { goToCheckout }
        />
        )
}

{
    currentPage === "checkout" && (
        <CheckoutPage
            token={ token }
    onOrderPlaced = { handleOrderPlaced }
        />
        )
}

{
    currentPage === "orders" && (
        <OrdersPage token={ token } />
        )
}
</main>

{
    selectedProduct && (
        <VariantPicker
          product={ selectedProduct }
    variants = { availableVariants }
    onAddToCart = { addSelectedVariantToCart }
    onClose = { closeVariantPicker }
        />
      )
}
</div>
  );
}

export default App;