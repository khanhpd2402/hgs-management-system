import { Bell } from "lucide-react";
import { Button } from "@/components/ui/button";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Avatar, AvatarFallback, AvatarImage } from "@/components/ui/avatar";
import { useAcademicYears } from "@/services/common/queries";
import { useEffect, useState } from "react";
import { useNavigate } from "react-router";
import PropTypes from "prop-types";
import {
  DropdownMenu,
  DropdownMenuContent,
  DropdownMenuItem,
  DropdownMenuTrigger,
} from "../ui/dropdown-menu";

const Header = ({ setCurrentYear }) => {
  const navigate = useNavigate();
  const academicYears = useAcademicYears();
  const [selectedYear, setSelectedYear] = useState(null);

  useEffect(() => {
    if (academicYears.data && academicYears.data.length > 0) {
      const currentYear = academicYears.data[0];
      setSelectedYear(currentYear);
      setCurrentYear(currentYear);
      sessionStorage.setItem(
        "currentAcademicYear",
        JSON.stringify(currentYear),
      );
    }
  }, [academicYears.data]);

  const handleLogout = () => {
    localStorage.removeItem("token");
    navigate("/login");
  };

  return (
    <>
      <div className="flex h-20 w-full items-center justify-between bg-[url('/banner.jpg')] bg-cover bg-center px-6">
        <div className="flex items-center gap-4">
          <img src="/logo.png" alt="School Logo" className="h-16 w-16" />
          <div className="text-white">
            <h1 className="text-2xl font-bold">TRƯỜNG THCS HẢI GIANG</h1>
            <p className="text-sm">Hải Giang - Hải Hậu - Nam Định</p>
          </div>
        </div>
      </div>
      <header className="flex h-14 items-center border-b px-4 lg:px-6">
        <div className="flex flex-1 items-center justify-between">
          <div className="font-medium">TRƯỜNG THCS HẢI GIANG</div>

          <div className="flex items-center gap-4">
            <Select
              value={selectedYear?.academicYearID}
              onValueChange={(value) => {
                const year = academicYears.data.find(
                  (y) => y.academicYearID === value,
                );
                setSelectedYear(year);
                setCurrentYear(year);

                sessionStorage.setItem(
                  "currentAcademicYear",
                  JSON.stringify(year),
                );
              }}
            >
              <SelectTrigger className="w-[130px]">
                <SelectValue>{selectedYear?.yearName}</SelectValue>
              </SelectTrigger>
              <SelectContent>
                {academicYears?.data?.map((item) => (
                  <SelectItem
                    key={item.academicYearID}
                    value={item.academicYearID}
                  >
                    {item.yearName}
                  </SelectItem>
                ))}
              </SelectContent>
            </Select>

            <Button variant="ghost" size="icon">
              <Bell className="h-5 w-5" />
            </Button>

            <div className="flex items-center gap-2">
              <DropdownMenu>
                <DropdownMenuTrigger asChild>
                  <Avatar className="cursor-pointer">
                    <AvatarImage src="" />
                    <AvatarFallback>HG</AvatarFallback>
                  </Avatar>
                </DropdownMenuTrigger>
                <DropdownMenuContent align="end">
                  <DropdownMenuItem onClick={handleLogout}>
                    Đăng xuất
                  </DropdownMenuItem>
                </DropdownMenuContent>
              </DropdownMenu>
            </div>
          </div>
        </div>
      </header>
    </>
  );
};
Header.propTypes = {
  setCurrentYear: PropTypes.func,
};

export default Header;
