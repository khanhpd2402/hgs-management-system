import React, { useState, useEffect } from 'react';
import { Button } from "@/components/ui/button";
import { Download, Upload } from 'lucide-react';
import { format } from 'date-fns';
import { getSemesterByYear } from '../../../services/schedule/api';
import { useAcademicYears } from '@/services/common/queries';
import toast from 'react-hot-toast';

// Component FilterSelect
const FilterSelect = ({ label, value, onChange, options, disabled }) => (
  <div className="grid gap-2">
    <label className="text-sm font-medium text-gray-700">{label}</label>
    <select
      value={value || ''}
      onChange={onChange}
      disabled={disabled}
      className="border rounded p-2 text-sm focus:ring-blue-500 focus:border-blue-500 disabled:bg-gray-100"
    >
      <option value="">Chọn {label.toLowerCase()}</option>
      {options.map(option => (
        <option key={option.value} value={option.value}>{option.label}</option>
      ))}
    </select>
  </div>
);

const ImportSchedule = ({ onClose }) => {
  const { data: academicYears = [], isLoading: academicYearsLoading } = useAcademicYears();
  const [file, setFile] = useState(null);
  const [academicYear, setAcademicYear] = useState('');
  const [semesters, setSemesters] = useState([]);
  const [semesterId, setSemesterId] = useState('');
  const [effectiveDateString, setEffectiveDateString] = useState('');
  const [isLoading, setIsLoading] = useState(false);

  useEffect(() => {
    const fetchSemesters = async () => {
      if (academicYear) {
        try {
          const semesterData = await getSemesterByYear(academicYear);
          setSemesters(semesterData || []);
          setSemesterId('');
        } catch (error) {
          console.error('Lỗi khi lấy danh sách học kỳ:', error);
          toast.error('Không thể lấy danh sách học kỳ');
        }
      } else {
        setSemesters([]);
        setSemesterId('');
      }
    };
    fetchSemesters();
  }, [academicYear]);

  const handleFileChange = (e) => {
    const selectedFile = e.target.files[0];
    if (selectedFile && selectedFile.type === 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet') {
      setFile(selectedFile);
    } else {
      toast.error('Vui lòng chọn file Excel (.xlsx)');
      setFile(null);
    }
  };

  const handleUpload = async () => {
    if (!file) {
      toast.error('Vui lòng chọn một file để tải lên');
      return;
    }
    if (!academicYear) {
      toast.error('Vui lòng chọn năm học');
      return;
    }
    if (!semesterId) {
      toast.error('Vui lòng chọn học kỳ');
      return;
    }
    if (!effectiveDateString) {
      toast.error('Vui lòng chọn ngày hiệu lực');
      return;
    }

    setIsLoading(true);
    const formData = new FormData();
    formData.append('file', file);
    formData.append('semesterId', semesterId);
    const formattedDate = format(new Date(effectiveDateString), 'dd/MM/yyyy');
    formData.append('effectiveDateString', formattedDate);

    try {
      const response = await fetch('https://localhost:8386/api/Timetables/import', {
        method: 'POST',
        headers: {
          'accept': 'application/json;odata.metadata=minimal;odata.streaming=true',
        },
        body: formData,
      });

      if (!response.ok) {
        const errorData = await response.json();
        console.error('Lỗi dữ liệu import:', errorData);
        throw new Error(errorData.message || 'Lỗi khi tải lên thời khóa biểu');
      }

      const result = await response.json();
      console.log('Kết quả từ API:', result);
      toast.success('Tải lên thời khóa biểu thành công');
      onClose();
    } catch (error) {
      console.error('Lỗi khi gọi API:', error);
      toast.error(error.message || 'Lỗi khi tải lên thời khóa biểu');
    } finally {
      setIsLoading(false);
    }
  };

  return (
    <div className="p-4">
      <div className="flex justify-between items-center mb-4">
        <h2 className="text-lg font-semibold text-gray-800">Nhập thông tin từ Excel</h2>
      </div>
      <div className="mb-4">
        <a
          href="/path/to/sample-excel.xlsx"
          download
          className="text-blue-500 hover:underline flex items-center gap-2"
        >
          <Download size={16} />
          Tải file excel mẫu
        </a>
      </div>
      <div className="grid gap-4 mb-4">
        <FilterSelect
          label="Năm học"
          value={academicYear}
          onChange={(e) => setAcademicYear(parseInt(e.target.value))}
          options={academicYears.map(year => ({
            value: year.academicYearID,
            label: `${year.yearName} -- ${year.academicYearID}`
          }))}
          disabled={academicYearsLoading}
        />
        <FilterSelect
          label="Học kỳ"
          value={semesterId}
          onChange={(e) => setSemesterId(parseInt(e.target.value))}
          options={semesters.map(semester => ({
            value: semester.semesterID,
            label: `${semester.semesterName} -- ${semester.semesterID}`
          }))}
          disabled={!academicYear || semesters.length === 0}
        />
        <div className="grid gap-2">
          <label className="text-sm font-medium text-gray-700">Ngày hiệu lực</label>
          <input
            type="date"
            value={effectiveDateString}
            onChange={(e) => setEffectiveDateString(e.target.value)}
            className="border rounded p-2 text-sm focus:ring-blue-500 focus:border-blue-500"
            disabled={!semesterId}
          />
        </div>
      </div>
      <div className="border-2 border-dashed border-gray-300 rounded-lg p-6 text-center">
        <label htmlFor="file-upload" className="cursor-pointer">
          <div className="flex flex-col items-center gap-2">
            <Upload size={24} className="text-gray-500" />
            <p className="text-gray-600">Thêm File hoặc kéo và thả</p>
          </div>
          <input
            id="file-upload"
            type="file"
            accept=".xlsx"
            onChange={handleFileChange}
            className="hidden"
          />
        </label>
        {file && (
          <p className="mt-2 text-sm text-gray-700">
            File đã chọn: {file.name}
          </p>
        )}
      </div>
      <div className="flex justify-end gap-2 mt-6">
        <Button
          variant="outline"
          onClick={onClose}
          className="border-gray-300 text-gray-700 hover:bg-gray-100"
          disabled={isLoading}
        >
          Đóng
        </Button>
        <Button
          onClick={handleUpload}
          className="bg-blue-500 text-white hover:bg-blue-600"
          disabled={isLoading || !file || !academicYear || !semesterId || !effectiveDateString}
        >
          {isLoading ? 'Đang tải lên...' : 'Tải lên'}
        </Button>
      </div>
    </div>
  );
};

export default ImportSchedule;