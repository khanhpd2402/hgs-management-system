import { useState, useEffect } from "react";
import { Checkbox } from "@/components/ui/checkbox";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogFooter,
} from "@/components/ui/dialog";
import { ScrollArea } from "@/components/ui/scroll-area";
import { Check } from "lucide-react";
import PropTypes from "prop-types";

const ColumnConfigModal = ({
  isOpen,
  onClose,
  columns,
  selectedColumns,
  onSave,
}) => {
  const [localSelectedColumns, setLocalSelectedColumns] =
    useState(selectedColumns);

  // Reset local state when the modal opens or selected columns change
  useEffect(() => {
    setLocalSelectedColumns(selectedColumns);
  }, [selectedColumns, isOpen]);

  const handleToggleColumn = (columnId) => {
    setLocalSelectedColumns((prev) => {
      if (prev.includes(columnId)) {
        return prev.filter((id) => id !== columnId);
      } else {
        return [...prev, columnId];
      }
    });
  };

  const handleSave = () => {
    onSave(localSelectedColumns);
    onClose();
  };

  return (
    <Dialog open={isOpen} onOpenChange={onClose}>
      <DialogContent className="sm:max-w-md">
        <DialogHeader>
          <DialogTitle>Cấu hình cột hiển thị</DialogTitle>
        </DialogHeader>
        <ScrollArea className="max-h-[300px] pr-4">
          <div className="space-y-3 py-2">
            {columns.map((column) => (
              <div key={column.id} className="flex items-center space-x-2">
                <Checkbox
                  id={column.id}
                  checked={localSelectedColumns.includes(column.id)}
                  onCheckedChange={() => handleToggleColumn(column.id)}
                />
                <label
                  htmlFor={column.id}
                  className="cursor-pointer text-sm leading-none font-medium peer-disabled:cursor-not-allowed peer-disabled:opacity-70"
                >
                  {column.label}
                </label>
              </div>
            ))}
          </div>
        </ScrollArea>
        <DialogFooter>
          <Button variant="outline" onClick={onClose}>
            Hủy
          </Button>
          <Button onClick={handleSave} className="gap-1">
            <Check className="h-4 w-4" /> Lưu
          </Button>
        </DialogFooter>
      </DialogContent>
    </Dialog>
  );
};

ColumnConfigModal.propTypes = {
  isOpen: PropTypes.bool.isRequired,
  onClose: PropTypes.func.isRequired,
  columns: PropTypes.array.isRequired,
  selectedColumns: PropTypes.array.isRequired,
  onSave: PropTypes.func.isRequired,
};

export default ColumnConfigModal;
