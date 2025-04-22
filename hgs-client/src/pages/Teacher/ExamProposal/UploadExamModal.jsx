import React, { useState } from "react";
import { Input } from "@/components/ui/input";
import {
  Select,
  SelectContent,
  SelectItem,
  SelectTrigger,
  SelectValue,
} from "@/components/ui/select";
import { Label } from "@/components/ui/label";
import { Button } from "@/components/ui/button";
import {
  Dialog,
  DialogTrigger,
  DialogContent,
  DialogTitle,
} from "@/components/ui/dialog";
import { useGradeLevels, useSubjects } from "@/services/common/queries";

// ... existing code ...

function UploadExamModal() {
  const [grade, setGrade] = useState("");
  const [subject, setSubject] = useState("");
  const [title, setTitle] = useState("");
  const [file, setFile] = useState(null);

  const gradeLevelQuery = useGradeLevels();
  const gradeLevels = gradeLevelQuery.data || [];
  console.log(gradeLevels);
  const subjectQuery = useSubjects();
  const subjects = subjectQuery.data || [];
  console.log(subjects);

  const handleSubmit = (e) => {
    e.preventDefault();
    const formData = new FormData();
    formData.append("grade", grade);
    formData.append("subject", subject);
    formData.append("title", title);
    formData.append("file", file);

    // TODO: Replace with your API call
    console.log({
      grade,
      subject,
      title,
      file,
    });
  };

  return (
    <Dialog>
      <DialogTrigger asChild>
        <Button variant="outline">Tải lên đề thi</Button>
      </DialogTrigger>
      <DialogContent className="max-w-lg">
        <DialogTitle className="mb-4 text-center">Tải lên đề thi</DialogTitle>
        <form onSubmit={handleSubmit} className="space-y-5">
          <div>
            <Label htmlFor="grade" className="mb-1 block">
              Khối lớp
            </Label>
            <Select value={grade} onValueChange={setGrade}>
              <SelectTrigger id="grade">
                <SelectValue placeholder="Chọn khối lớp" />
              </SelectTrigger>
              <SelectContent>
                {gradeLevels.map((gradeLevel) => (
                  <SelectItem
                    value={gradeLevel.gradeLevelId + ""}
                    key={gradeLevel.gradeLevelId}
                  >
                    {gradeLevel.gradeName}
                  </SelectItem>
                ))}

                {/* Add more grades if needed */}
              </SelectContent>
            </Select>
          </div>
          <div>
            <Label htmlFor="subject" className="mb-1 block">
              Môn học
            </Label>
            <Select value={subject} onValueChange={setSubject}>
              <SelectTrigger id="subject">
                <SelectValue placeholder="Chọn môn học" />
              </SelectTrigger>
              <SelectContent>
                {subjects.map((subject) => (
                  <SelectItem
                    value={subject.subjectID + ""}
                    key={subject.subjectID}
                  >
                    {subject.subjectName}
                  </SelectItem>
                ))}

                {/* Add more subjects if needed */}
              </SelectContent>
            </Select>
          </div>
          <div>
            <Label htmlFor="title" className="mb-1 block">
              Tiêu đề
            </Label>
            <Input
              id="title"
              type="text"
              value={title}
              onChange={(e) => setTitle(e.target.value)}
              placeholder="Nhập tiêu đề đề thi"
              required
            />
          </div>
          <div>
            <Label htmlFor="file" className="mb-1 block">
              File đề thi
            </Label>
            <Input
              id="file"
              type="file"
              onChange={(e) => setFile(e.target.files[0])}
              required
            />
          </div>
          <Button type="submit" className="w-full">
            Tải lên
          </Button>
        </form>
      </DialogContent>
    </Dialog>
  );
}

export default UploadExamModal;
