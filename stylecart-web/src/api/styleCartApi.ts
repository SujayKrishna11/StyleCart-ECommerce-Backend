import type {
    Cart,
    CheckoutRequest,
    LoginResponse,
    Order,
    Product,
    ProductVariant,
} from "../types/models";

const apiUrl = "https://localhost:65125/api";

async function sendRequest<T>(
    endpoint: string,
    options: RequestInit = {},
    token?: string,
): Promise<T> {
    const headers = new Headers(options.headers);

    if (options.body) {
        headers.set("Content-Type", "application/json");
    }

    if (token) {
        headers.set("Authorization", `Bearer ${token}`);
    }

    const response = await fetch(`${apiUrl}${endpoint}`, {
        ...options,
        headers,
    });

    if (!response.ok) {
        const errorMessage = await response.text();

        throw new Error(
            errorMessage || `Request failed with status ${response.status}.`,
        );
    }

    if (response.status === 204) {
        return undefined as T;
    }

    return response.json() as Promise<T>;
}

export function getProducts(token?: string) {
    return sendRequest<Product[]>("/Products", {}, token);
}

export function getAvailableProducts() {
    return sendRequest<Product[]>("/Products/available");
}

export function getProductVariants(productId: number, token: string) {
    return sendRequest<ProductVariant[]>(
        `/ProductVariants?productId=${productId}`,
        {},
        token,
    );
}

export function login(email: string, password: string) {
    return sendRequest<LoginResponse>("/Auth/login", {
        method: "POST",
        body: JSON.stringify({
            email,
            password,
        }),
    });
}

export function register(
    firstName: string,
    lastName: string,
    email: string,
    password: string,
) {
    return sendRequest<LoginResponse>("/Auth/register", {
        method: "POST",
        body: JSON.stringify({
            firstName,
            lastName,
            email,
            password,
        }),
    });
}

export function getCart(token: string) {
    return sendRequest<Cart>("/Cart", {}, token);
}

export function addItemToCart(
    productVariantId: number,
    quantity: number,
    token: string,
) {
    return sendRequest(
        "/Cart/items",
        {
            method: "POST",
            body: JSON.stringify({
                productVariantId,
                quantity,
            }),
        },
        token,
    );
}

export function updateCartItem(
    cartItemId: number,
    quantity: number,
    token: string,
) {
    return sendRequest(
        `/Cart/items/${cartItemId}`,
        {
            method: "PUT",
            body: JSON.stringify({
                quantity,
            }),
        },
        token,
    );
}

export function removeCartItem(cartItemId: number, token: string) {
    return sendRequest(
        `/Cart/items/${cartItemId}`,
        {
            method: "DELETE",
        },
        token,
    );
}

export function checkout(data: CheckoutRequest, token: string) {
    return sendRequest<Order>(
        "/Orders/checkout",
        {
            method: "POST",
            body: JSON.stringify(data),
        },
        token,
    );
}

export function getMyOrders(token: string) {
    return sendRequest<Order[]>("/Orders/my-orders", {}, token);
}