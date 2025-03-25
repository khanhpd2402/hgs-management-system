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

const Header = ({ setCurrentYear }) => {
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

  return (
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
            <span>Trường THCS Hải Giang</span>
            <Avatar>
              <AvatarImage src="" />
              <AvatarFallback>HG</AvatarFallback>
            </Avatar>
          </div>
        </div>
      </div>
    </header>
  );
};

export default Header;
