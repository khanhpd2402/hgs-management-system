// import Cart from "./components/Cart";
// import ChangeQtyButtons from "./components/ChangeQtyButtons";
// import { Button } from "./components/ui/button";
// import {
//   Card,
//   CardContent,
//   CardFooter,
//   CardHeader,
// } from "./components/ui/card";
// import User from "./components/User";
// import { PRODUCTS_DATA } from "./lib/mockData";
// import { useStore } from "./stores/store";
import Todos from "./components/Todos";

const App = () => {
  // const addProduct = useStore((state) => state.addProduct);
  // const cartProducts = useStore((state) => state.products);
  return (
    // <main className="dark bg-background mx-auto mt-2 h-screen max-w-sm space-y-2">
    //   <div className="flex justify-between">
    //     <User />
    //     <Cart />
    //   </div>
    //   <h1 className="text-2xl">Products:</h1>
    //   <div className="space-y-2">
    //     {PRODUCTS_DATA.map((product) => (
    //       <Card key={product.id}>
    //         <CardHeader>{product.title}</CardHeader>

    //         <CardContent>{product.price}$</CardContent>
    //         <CardFooter>
    //           {cartProducts.find((item) => item.id === product.id) ? (
    //             <ChangeQtyButtons productId={product.id} />
    //           ) : (
    //             <Button onClick={() => addProduct(product)}>Add to cart</Button>
    //           )}
    //         </CardFooter>
    //       </Card>
    //     ))}
    //   </div>
    // </main>
    <Todos />
  );
};

export default App;
