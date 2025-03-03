import { Button } from "./ui/button";
import { UserIcon } from "lucide-react";
import { useShallow } from "zustand/react/shallow";
import { useStore } from "@/stores/store";
import { Popover, PopoverContent, PopoverTrigger } from "./ui/popover";
import { Label } from "@radix-ui/react-label";
import { Input } from "./ui/input";
import { useEffect } from "react";

const User = () => {
  const { setAddress, address, fullName, userName, fetchUser } = useStore(
    useShallow((state) => ({
      setAddress: state.setAddress,
      address: state.address,
      fullName: state.fullName,
      userName: state.userName,
      fetchUser: state.fetchUser,
    })),
  );

  useEffect(() => {
    async function fetchData() {
      await fetchUser();
    }

    fetchData();
  }, []);

  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button variant="secondary" size="icon">
          <UserIcon />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-96 space-y-2 overflow-y-scroll">
        <div className="flex items-center gap-2 text-lg">
          <p>{userName}</p>
          <p className="text-sm">{fullName}</p>
        </div>
        <Label>Your Address</Label>
        <Input
          id="address"
          value={address}
          onChange={(e) => setAddress(e.target.value)}
        />
      </PopoverContent>
    </Popover>
  );
};

export default User;
