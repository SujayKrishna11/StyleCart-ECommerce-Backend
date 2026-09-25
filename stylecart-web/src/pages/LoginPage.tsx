import type { FormEvent } from "react";
import { useState } from "react";
import { login } from "../api/styleCartApi";
import type { LoginResponse } from "../types/models";

type LoginPageProps = {
    onLogin: (response: LoginResponse) => void;
};

function LoginPage({ onLogin }: LoginPageProps) {
    const [error, setError] = useState("");
    const [isSubmitting, setIsSubmitting] = useState(false);

    async function handleSubmit(event: FormEvent<HTMLFormElement>) {
        event.preventDefault();

        const formData = new FormData(event.currentTarget);
        const email = String(formData.get("email"));
        const password = String(formData.get("password"));

        try {
            setError("");
            setIsSubmitting(true);

            const response = await login(email, password);
            onLogin(response);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Login failed. Please try again.");
            }
        } finally {
            setIsSubmitting(false);
        }
    }

    return (
        <section className= "form-page" >
        <div className="form-card" >
            <p className="eyebrow" > Customer access </p>
                < h1 > Login </h1>
                < p > Use your registered email address and password.</p>

                    < form onSubmit = { handleSubmit } >
                        <label>
                        Email
                        < input name = "email" type = "email" required />
                            </label>

                            <label>
    Password
        < input name = "password" type = "password" required />
            </label>

    { error && <p className="error-message" > { error } </p> }

    <button type="submit" disabled = { isSubmitting } >
    { isSubmitting? "Logging in...": "Login" }
        </button>
        </form>
        </div>
        </section>
  );
}

export default LoginPage;