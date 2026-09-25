import type { Product, ProductVariant } from "../types/models";

type VariantPickerProps = {
    product: Product;
    variants: ProductVariant[];
    onAddToCart: (variantId: number) => void;
    onClose: () => void;
};

function VariantPicker({
    product,
    variants,
    onAddToCart,
    onClose,
}: VariantPickerProps) {
    return (
        <div className= "modal-background" >
        <section className="variant-picker" >
            <button
          className="close-button"
    type = "button"
    onClick = { onClose }
        >
          ×
    </button>

        < p className = "eyebrow" > Choose a variant </p>
            < h2 > { product.name } </h2>

            < div className = "variant-list" >
            {
                variants.map((variant) => (
                    <button
              className= "variant-button"
              key = { variant.id }
              type = "button"
              onClick = {() => onAddToCart(variant.id)}
                >
                <span>
                { variant.color } · { variant.size }
    </span>

        <strong>₹{ variant.price } </strong>
            </button>
          ))
}
</div>
    </section>
    </div>
  );
}

export default VariantPicker;