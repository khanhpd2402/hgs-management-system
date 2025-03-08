import { useStore } from "@/stores/store";
import { useShallow } from "zustand/react/shallow";
import { Button } from "./ui/button";
import { Minus, Plus } from "lucide-react";
import { useEffect } from "react";
import PropTypes from "prop-types";

const ChangeQtyButtons = ({ productId }) => {
  const { incQty, decQty, setTotal, product } = useStore(
    useShallow((state) => ({
      incQty: state.incQty,
      decQty: state.decQty,
      setTotal: state.setTotal,
      products: state.products,
      product: state.getProductById(productId),
    })),
  );

  console.log(product);
  useEffect(() => {
    const unSub = useStore.subscribe(
      (state) => state.products,
      (products) => {
        setTotal(
          products.reduce(
            (acc, product) => acc + product.price * product.qty,
            0,
          ),
        );
      },
      { fireImmediately: true },
    );
    return unSub;
  }, [setTotal]);

  return (
    <>
      {product && (
        <div className="flex items-center gap-2">
          <Button onClick={() => decQty(product.id)}>
            <Minus />
          </Button>
          <span>{product.qty}</span>
          <Button onClick={() => incQty(product.id)}>
            <Plus />
          </Button>
        </div>
      )}
    </>
  );
};

ChangeQtyButtons.propTypes = {
  productId: PropTypes.number.isRequired,
};

export default ChangeQtyButtons;
