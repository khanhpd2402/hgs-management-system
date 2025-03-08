import { format } from "date-fns";
import { CalendarIcon } from "lucide-react";

import { cn } from "@/lib/utils";
import { Button } from "@/components/ui/button";
import { Calendar } from "@/components/ui/calendar";
import {
  Popover,
  PopoverContent,
  PopoverTrigger,
} from "@/components/ui/popover";
import PropTypes from "prop-types";

const DatePicker = ({ value, onSelect }) => {
  return (
    <Popover>
      <PopoverTrigger asChild>
        <Button
          variant="outline"
          className={cn(
            "w-[150px] justify-between font-normal",
            !value && "text-gray-400",
          )}
          readOnly // Chặn nhập trực tiếp
        >
          {value ? format(value, "dd/MM/yyyy") : "dd/mm/yyyy"}
          <CalendarIcon className="h-4 w-4 text-gray-500" />
        </Button>
      </PopoverTrigger>
      <PopoverContent className="w-auto p-0">
        <Calendar
          mode="single"
          selected={value}
          onSelect={onSelect}
          initialFocus
        />
      </PopoverContent>
    </Popover>
  );
};

DatePicker.propTypes = {
  value: PropTypes.instanceOf(Date),
  onSelect: PropTypes.func,
};

export default DatePicker;
