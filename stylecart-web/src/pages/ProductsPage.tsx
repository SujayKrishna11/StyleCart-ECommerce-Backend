import { useEffect, useState } from "react";
import { getAvailableProducts } from "../api/styleCartApi";
import ProductCard from "../components/ProductCard";
import type { Product } from "../types/models";

type ProductsPageProps = {
    onAddToCart: (product: Product) => void;
};

function ProductsPage({ onAddToCart }: ProductsPageProps) {
    const [products, setProducts] = useState<Product[]>([]);
    const [loading, setLoading] = useState(true);
    const [error, setError] = useState("");

    useEffect(() => {
        loadProducts();
    }, []);

    async function loadProducts() {
        try {
            setLoading(true);
            setError("");

            const productData = await getAvailableProducts();
            setProducts(productData);
        } catch (requestError) {
            if (requestError instanceof Error) {
                setError(requestError.message);
            } else {
                setError("Could not load products.");
            }
        } finally {
            setLoading(false);
        }
    }

    return (
        <section>
        <div className= "page-heading" >
        <p className="eyebrow" > Fashion store </p>
            < h1 > Products </h1>
            < p > Browse products that are currently available to purchase.</p>
                </div>

    { loading && <p>Loading products...</p> }

    { error && <p className="error-message" > { error } </p> }

    {
        !loading && !error && products.length === 0 && (
            <p>No products are available to purchase right now.</p>
      )
    }

    <div className="product-grid" >
    {
        products.map((product) => (
            <ProductCard
            key= { product.id }
            product = { product }
            onAddToCart = { onAddToCart }
            />
        ))
    }
        </div>
        </section>
  );
}

export default ProductsPage;