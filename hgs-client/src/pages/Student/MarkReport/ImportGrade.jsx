import React, { useState } from 'react';
import { Button } from "@/components/ui/button";
import {
    Dialog,
    DialogContent,
    DialogHeader,
    DialogTitle,
    DialogTrigger,
} from "@/components/ui/dialog";
import { Input } from "@/components/ui/input";
import { Label } from "@/components/ui/label";
import axios from 'axios';
import toast from 'react-hot-toast';

const ImportGrade = ({ classId, subjectId, semesterId }) => {

    console.log(classId, subjectId, semesterId)
    const [file, setFile] = useState(null);
    const [loading, setLoading] = useState(false);

    const handleFileChange = (e) => {
        const selectedFile = e.target.files[0];
        if (selectedFile && selectedFile.type === "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet") {
            setFile(selectedFile);
        } else {
            toast.error("Vui lòng chọn file Excel (.xlsx)");
            e.target.value = null;
        }
    };

    const handleUpload = async () => {
        if (!file) {
            toast.error("Vui lòng chọn file Excel");
            return;
        }

        if (!classId || !subjectId || !semesterId) {
            toast.error("Vui lòng chọn đầy đủ thông tin lớp học và môn học");
            return;
        }

        setLoading(true);
        const formData = new FormData();
        formData.append("file", file);

        try {
            const response = await axios.post(
                `https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Grades/import/${classId}/${subjectId}/${semesterId}`,
                formData,
                {
                    headers: {
                        'Content-Type': 'multipart/form-data',
                    },
                }
            );

            const result = response.data;

            if (result.isSuccess) {
                if (result.updatedRecords > 0) {
                    toast.success(`Nhập điểm thành công! Đã cập nhật ${result.updatedRecords} điểm.`);
                } else {
                    toast.success(result.message);
                }
            } else {
                if (result.errors && result.errors.length > 0) {
                    toast.error(`Lỗi: ${result.errors.join(', ')}`);
                } else {
                    toast.error(result.message || "Có lỗi xảy ra khi nhập điểm");
                }
            }
            setFile(null);
        } catch (error) {
            console.error('Error uploading file:', error);
            toast.error("Có lỗi xảy ra khi nhập điểm. Vui lòng thử lại!");
        } finally {
            setLoading(false);
        }
    };

    const handleDownloadTemplate = () => {
        const link = document.createElement('a');
        link.href = '/template/import-grade-template.xlsx';
        link.download = 'import-grade-template.xlsx';
        document.body.appendChild(link);
        link.click();
        document.body.removeChild(link);
    };

    return (
        <Dialog>
            <DialogTrigger asChild>
                <Button className="bg-blue-600 hover:bg-blue-700">
                    Nhập điểm từ Excel
                </Button>
            </DialogTrigger>
            <DialogContent className="sm:max-w-[425px]">
                <DialogHeader>
                    <DialogTitle>Nhập thông tin từ Excel</DialogTitle>
                </DialogHeader>
                <div className="grid gap-4 py-4">

                    <div className="flex flex-col gap-4">
                        <Label htmlFor="file-upload">Hoặc kéo và thả</Label>
                        <Input
                            id="file-upload"
                            type="file"
                            accept=".xlsx"
                            onChange={handleFileChange}
                            disabled={loading}
                        />
                    </div>
                    <Button
                        onClick={handleUpload}
                        disabled={!file || loading}
                        className="bg-blue-600 hover:bg-blue-700"
                    >
                        {loading ? "Đang tải lên..." : "Tải lên"}
                    </Button>
                </div>
            </DialogContent>
        </Dialog>
    );
};

export default ImportGrade;
