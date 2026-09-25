import type { Product } from "../types/models";

type ProductCardProps = {
    product: Product;
    onAddToCart: (product: Product) => void;
};

function ProductCard({ product, onAddToCart }: ProductCardProps) {
    return (
        <article className= "product-card" >
        <div className="product-placeholder" >
        { product.brand || "StyleCart" }
            </div>

            < div className = "product-details" >
                <p className="product-brand" > { product.brand || "StyleCart" } </p>

                    < h2 > { product.name } </h2>

                    < p className = "product-description" >
                    { product.description || "No description available." }
                        </p>

                        < div className = "product-footer" >
                            <strong>₹{ product.basePrice } </strong>

                                < button type = "button" onClick = {() => onAddToCart(product)
}>
    Add to cart
        </button>
        </div>
        </div>
        </article>
  );
}

export default ProductCard;