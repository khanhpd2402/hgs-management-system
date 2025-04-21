import React, { useState } from "react";
import UploadExamModal from "./UploadExamModal";
import { useGradeLevels } from "@/services/common/queries";
function UploadExam() {
  const [exams, setExams] = useState([
    {
      id: 1,
      grade: "10",
      subject: "Toán",
      title: "Đề thi giữa kỳ 1",
      filename: "de_giua_ky_1.pdf",
    },
    {
      id: 2,
      grade: "11",
      subject: "Vật lý",
      title: "Đề thi cuối kỳ",
      filename: "de_cuoi_ky.pdf",
    },
  ]);

  const gradeLevelQuery = useGradeLevels();
  const gradeLevels = gradeLevelQuery.data || [];
  console.log(gradeLevels);

  // Handler to add a new exam from the upload form
  const handleAddExam = (exam) => {
    setExams([
      ...exams,
      {
        ...exam,
        id: exams.length + 1,
      },
    ]);
  };

  return (
    <div>
      <div className="flex items-center justify-between">
        <h2 className="mt-4 mb-6 text-3xl font-bold">Đề thi đã tải lên</h2>
        <UploadExamModal />
      </div>
      {/* Uncomment the next line if you want to show the upload form here */}
      {/* <UploadExamForm onAddExam={handleAddExam} /> */}
      <div className="overflow-x-auto">
        <table className="min-w-full border text-sm">
          <thead>
            <tr className="bg-gray-100">
              <th className="border px-4 py-2">#</th>
              <th className="border px-4 py-2">Khối lớp</th>
              <th className="border px-4 py-2">Môn học</th>
              <th className="border px-4 py-2">Tiêu đề</th>
              <th className="border px-4 py-2">Tên file</th>
            </tr>
          </thead>
          <tbody>
            {exams.map((exam, idx) => (
              <tr key={exam.id}>
                <td className="border px-4 py-2 text-center">{idx + 1}</td>
                <td className="border px-4 py-2 text-center">{exam.grade}</td>
                <td className="border px-4 py-2 text-center">{exam.subject}</td>
                <td className="border px-4 py-2">{exam.title}</td>
                <td className="border px-4 py-2">{exam.filename}</td>
              </tr>
            ))}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default UploadExam;
