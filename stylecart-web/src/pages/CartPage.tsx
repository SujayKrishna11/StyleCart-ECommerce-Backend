import type { Cart } from "../types/models";

type CartPageProps = {
    cart: Cart | null;
    onIncreaseQuantity: (cartItemId: number, quantity: number) => void;
    onDecreaseQuantity: (cartItemId: number, quantity: number) => void;
    onRemoveItem: (cartItemId: number) => void;
    onCheckout: () => void;
};

function CartPage({
    cart,
    onIncreaseQuantity,
    onDecreaseQuantity,
    onRemoveItem,
    onCheckout,
}: CartPageProps) {
    if (!cart || cart.items.length === 0) {
        return (
            <section>
            <div className= "page-heading" >
            <p className="eyebrow" > Your selection </p>
                < h1 > Cart </h1>
                < p > Your cart is empty.</p>
                    </div>
                    </section>
    );
    }

    const total = cart.items.reduce(
        (sum, item) => sum + item.lineTotal,
        0,
    );

    return (
        <section>
        <div className= "page-heading" >
        <p className="eyebrow" > Your selection </p>
            < h1 > Cart </h1>
            </div>

            < div className = "cart-list" >
            {
                cart.items.map((item) => (
                    <article className= "cart-item" key = { item.id } >
                    <div>
                    <h2>{ item.productName } </h2>
                    <p>
                Color: { item.color } · Size: { item.size }
                </p>
                <p>₹{ item.unitPrice } each </p>
                </div>

                < div className = "cart-item-actions" >
                <div className="quantity-controls" >
                <button
                  type="button"
                  onClick = {() =>
                    onDecreaseQuantity(item.id, item.quantity)
                  }
    disabled = { item.quantity === 1 }
        >
                  −
    </button>

        < span > { item.quantity } </span>

        < button
    type = "button"
    onClick = {() =>
    onIncreaseQuantity(item.id, item.quantity)
}
                >
    +
    </button>
    </div>

    <strong>₹{ item.lineTotal } </strong>

        < button
className = "remove-button"
type = "button"
onClick = {() => onRemoveItem(item.id)}
              >
    Remove
    </button>
    </div>
    </article>
        ))}
</div>

    < div className = "cart-total" >
        <strong>Total: ₹{ total } </strong>

            < button type = "button" onClick = { onCheckout } >
                Proceed to checkout
                    </button>
                    </div>
                    </section>
  );
}

export default CartPage;