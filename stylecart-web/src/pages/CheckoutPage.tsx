import type { FormEvent } from "react";
import { useState } from "react";
import { checkout } from "../api/styleCartApi";
import type { CheckoutRequest, Order } from "../types/models";

type CheckoutPageProps = {
    token: string;
    onOrderPlaced: (order: Order) => void;
};

function CheckoutPage({
    token,
    onOrderPlaced,
}: CheckoutPageProps) {
    const [error, setError] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const formData = new FormData(event.currentTarget);

        const request: CheckoutRequest = {
            recipientName: String(formData.get("recipientName")),
            shippingAddressLine1: String(formData.get("shippingAddressLine1")),
            city: String(formData.get("city")),
            state: String(formData.get("state")),
            postalCode: String(formData.get("postalCode")),
            country: String(formData.get("country")),
            phoneNumber: String(formData.get("phoneNumber")),
        };

        try {
            setError("");
            setIsSubmitting(true);

            const order = await checkout(request, token);
            onOrderPlaced(order);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Could not place the order.");
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <section className= "form-page" >
        <div className="form-card" >
            <p className="eyebrow" > Final step </p>
                < h1 > Checkout </h1>
                < p > Enter the delivery details for your order.</p>

                    < form onSubmit = { handleSubmit } >
                    <label>
                    Recipient name
                        < input name = "recipientName" required />
                            </label>

                            <label>
    Address
        < input name = "shippingAddressLine1" required />
            </label>

            <label>
    City
        < input name = "city" required />
            </label>

            <label>
    State
        < input name = "state" required />
            </label>

            <label>
            Postal code
        < input name = "postalCode" required />
            </label>

            <label>
    Country
        < input name = "country" defaultValue = "India" required />
            </label>

            <label>
            Phone number
        < input name = "phoneNumber" required />
            </label>

    { error && <p className="error-message" > { error } </p> }

    <button type="submit" disabled = { isSubmitting } >
    { isSubmitting? "Placing order...": "Place order" }
        </button>
        </form>
        </div>
        </section>
  );
}

export default CheckoutPage;