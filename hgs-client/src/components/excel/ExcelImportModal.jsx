import { useState } from "react";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogContent,
  DialogHeader,
  DialogTitle,
  DialogTrigger,
} from "@/components/ui/dialog";
import { Label } from "@/components/ui/label";
import { RadioGroup, RadioGroupItem } from "@/components/ui/radio-group";
import { Upload, XCircle, FileText, Download } from "lucide-react";
import { axiosInstance } from "@/services/axios";

export default function ExcelImportModal({ type }) {
  const [file, setFile] = useState(null);
  const [importType, setImportType] = useState("update");
  const [dragging, setDragging] = useState(false);

  const submitFile = async (file) => {
    const response = await axiosInstance.post(`/${type}/import`, file);
    console.log(response);
  };

  // Fake danh sách file mẫu theo từng trang
  const sampleFiles = {
    employees:
      "https://docs.google.com/spreadsheets/d/1ULLJxX-ocO0y3zO2B8HCTq4_gdqBtenA/export?format=xlsx",
    students: "/files/student_template.xlsx",
    courses: "/files/course_template.xlsx",
  };

  const sampleFileUrl = sampleFiles[type] || "#";

  const handleFileChange = (event) => {
    const selectedFile = event.target.files[0];
    if (selectedFile) {
      setFile(selectedFile);
    }
  };

  const handleDrop = (event) => {
    event.preventDefault();
    setDragging(false);
    const droppedFile = event.dataTransfer.files[0];
    if (droppedFile) {
      setFile(droppedFile);
    }
  };

  const handleUpload = () => {
    if (file) {
      console.log("Uploading:", file);
      const formData = new FormData();
      formData.append("file", file);
      submitFile(formData);
      // TODO: Xử lý upload file lên server
    } else {
      alert("Vui lòng chọn file Excel!");
    }
  };

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="outline">Nhập dữ liệu từ Excel</Button>
      </DialogTrigger>
      <DialogContent className="max-w-md">
        <DialogHeader>
          <DialogTitle>Nhập thông tin từ Excel</DialogTitle>
        </DialogHeader>

        {/* Tải file mẫu */}
        <div className="flex items-center justify-between border-b pb-2">
          <a
            href={sampleFileUrl}
            download
            className="flex items-center gap-2 text-blue-600 hover:underline"
          >
            <Download className="h-5 w-5" />
            <span>Tải file excel mẫu</span>
          </a>
        </div>

        {file ? (
          // Nếu đã chọn file, chỉ hiển thị tên file
          <div className="flex items-center justify-between rounded-md border border-gray-300 p-3 text-gray-700">
            <div className="flex items-center gap-2">
              <FileText className="h-5 w-5 text-blue-600" />
              <span className="text-sm font-medium">{file.name}</span>
            </div>
            <XCircle
              className="h-5 w-5 cursor-pointer text-red-500"
              onClick={() => setFile(null)}
            />
          </div>
        ) : (
          // Nếu chưa chọn file, hiển thị vùng kéo thả
          <div
            className={`flex flex-col items-center gap-2 rounded-md border-2 border-dashed p-6 text-gray-600 transition ${
              dragging ? "border-blue-500 bg-blue-50" : "border-gray-300"
            }`}
            onDragOver={(e) => {
              e.preventDefault();
              setDragging(true);
            }}
            onDragLeave={() => setDragging(false)}
            onDrop={handleDrop}
          >
            <Upload className="h-10 w-10" />
            <Label htmlFor="file-upload" className="cursor-pointer font-medium">
              Thêm File
            </Label>
            <span className="text-sm">hoặc kéo và thả</span>
            <input
              id="file-upload"
              type="file"
              accept=".xlsx, .xls"
              className="hidden"
              onChange={handleFileChange}
            />
          </div>
        )}

        {/* Chọn loại import */}
        <RadioGroup
          value={importType}
          onValueChange={setImportType}
          className="space-y-2"
        >
          <Label className="flex items-center gap-2">
            <RadioGroupItem value="new" /> Thêm dữ liệu mới
          </Label>
          <Label className="flex items-center gap-2">
            <RadioGroupItem value="update" /> Cập nhật dữ liệu
          </Label>
        </RadioGroup>

        {/* Nút hành động */}
        <div className="flex justify-end gap-2">
          <Button variant="outline">Đóng</Button>
          <Button onClick={handleUpload} disabled={!file}>
            Tải lên
          </Button>
        </div>
      </DialogContent>
    </Dialog>
  );
}
