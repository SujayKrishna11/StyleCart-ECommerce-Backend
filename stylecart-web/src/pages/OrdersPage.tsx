import { useEffect, useState } from "react";
import { getMyOrders } from "../api/styleCartApi";
import type { Order } from "../types/models";

type OrdersPageProps = {
    token: string;
};

function OrdersPage({ token }: OrdersPageProps) {
    const [orders, setOrders] = useState<Order[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadOrders();
    }, []);

    async function loadOrders() {
        try {
            setLoading(true);
            setError("");

            const orderData = await getMyOrders(token);
            setOrders(orderData);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Could not load your orders.");
            }
        } finally {
            setLoading(false);
        }
    }

    return (
        <section>
        <div className= "page-heading" >
        <p className="eyebrow" > Purchase history </p>
            < h1 > My Orders </h1>
                </div>

    { loading && <p>Loading orders...</p> }

    { error && <p className="error-message" > { error } </p> }

    {
        !loading && !error && orders.length === 0 && (
            <p>You have not placed any orders yet.</p>
      )
    }

    <div className="order-list" >
    {
        orders.map((order) => (
            <article className= "order-card" key = { order.id } >
            <div className="order-summary" >
        <div>
        <h2>Order #{ order.id } </h2>
        < p > { new Date(order.orderDate).toLocaleString() } </p>
        <p>
                  Status: <strong>{ order.orderStatus } </strong>
        </p>
        <p>
                  Payment: <strong>{ order.paymentStatus } </strong>
        </p>
        </div>

        < strong className = "order-total" >₹{ order.totalAmount } </strong>
        </div>

        < div className = "order-items" >
        {
            order.items.map((item) => (
                <div className= "order-item" key = { item.id } >
                <span>
                { item.productName } — { item.variantDescription } ×{ " "}
                    { item.quantity }
                </span>

                <strong>₹{ item.lineTotal } </strong>
            </div>
            ))
    }
        </div>
        </article>
        ))
}
</div>
    </section>
  );
}

export default OrdersPage;