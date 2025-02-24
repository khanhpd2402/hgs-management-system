import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import PropTypes from "prop-types";

const PaginationControls = ({
  pageSize,
  setPageSize,
  totalItems,
  startIndex,
  endIndex,
}) => {
  return (
    <div className="flex items-center gap-4">
      {/* Chọn số lượng hiển thị */}
      <Select
        value={pageSize.toString()}
        onValueChange={(value) => setPageSize(Number(value))}
      >
        <SelectTrigger className="w-20">
          <SelectValue />
        </SelectTrigger>
        <SelectContent>
          {[5, 10, 20, 50].map((size) => (
            <SelectItem key={size} value={size.toString()}>
              {size}
            </SelectItem>
          ))}
        </SelectContent>
      </Select>

      {/* Hiển thị tổng số nhân viên & phạm vi */}
      <span>
        Tổng số: {totalItems}. Hiển thị {startIndex} - {endIndex}
      </span>
    </div>
  );
};

PaginationControls.propTypes = {
  pageSize: PropTypes.number.isRequired,
  setPageSize: PropTypes.func.isRequired,
  totalItems: PropTypes.number.isRequired,
  startIndex: PropTypes.number.isRequired,
  endIndex: PropTypes.number.isRequired,
};

export default PaginationControls;
