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
import { Upload, XCircle, FileText, Download } from "lucide-react";
import { useImportTeachers } from "@/services/teacher/mutation";

export default function TeacherImportExcel({ type }) {
  const [file, setFile] = useState(null);
  const [dragging, setDragging] = useState(false);
  const [isUploading, setIsUploading] = useState(false);
  const { mutate, isPending, error } = useImportTeachers();

  // Danh sách file mẫu
  const sampleFiles = {
    employees:
      "https://docs.google.com/spreadsheets/d/1ULLJxX-ocO0y3zO2B8HCTq4_gdqBtenA/export?format=xlsx",
    students: "/files/student_template.xlsx",
    courses: "/files/course_template.xlsx",
  };

  const sampleFileUrl = sampleFiles[type] || "#";

  // Khi chọn file từ input
  const handleFileChange = (event) => {
    const selectedFile = event.target.files[0];
    if (selectedFile) {
      setFile(selectedFile);
    }
  };

  // Khi kéo thả file vào vùng upload
  const handleDrop = (event) => {
    event.preventDefault();
    setDragging(false);
    const droppedFile = event.dataTransfer.files[0];
    if (droppedFile) {
      setFile(droppedFile);
    }
  };

  // Gửi file lên backend
  const handleUpload = () => {
    if (!file) {
      alert("Vui lòng chọn file Excel!");
      return;
    }

    setIsUploading(true);

    // Tạo FormData để gửi file lên backend
    const formData = new FormData();
    formData.append("file", file);

    mutate(formData, {
      onSuccess: () => {
        alert("Tải lên thành công!");
        setFile(null);
      },
      onError: (err) => {
        alert(`Lỗi: ${err.message || "Không thể tải lên file!"}`);
      },
      onSettled: () => {
        setIsUploading(false);
      },
    });
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
          // Hiển thị tên file khi đã chọn
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
          // Vùng kéo thả file
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

        {/* Nút hành động */}
        <div className="flex justify-end gap-2">
          <Button variant="outline">Đóng</Button>
          <Button onClick={handleUpload} disabled={!file || isUploading}>
            {isUploading ? "Đang tải lên..." : "Tải lên"}
          </Button>
        </div>

        {/* Hiển thị lỗi nếu có */}
        {error && <p className="text-sm text-red-500">{error.message}</p>}
      </DialogContent>
    </Dialog>
  );
}
