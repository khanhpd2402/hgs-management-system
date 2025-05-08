import React, { useState, useMemo, useCallback, useEffect } from 'react';

const ContactParents = () => {
  const [academicYears, setAcademicYears] = useState([]);
  const [selectedAcademicYear, setSelectedAcademicYear] = useState('');
  const [studentsData, setStudentsData] = useState([]);
  const [isLoading, setIsLoading] = useState({ years: false, students: false });
  const [error, setError] = useState(null);
  const [message, setMessage] = useState('');
  const [subject, setSubject] = useState('');
  const [isHtml, setIsHtml] = useState(false);
  const [selectedStudents, setSelectedStudents] = useState(new Set());
  const [filters, setFilters] = useState({ className: '', gender: '' });
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);
  const [isSending, setIsSending] = useState(false);
  const [sendError, setSendError] = useState(null);

  // Fetch academic years and students
  useEffect(() => {
    const fetchAcademicYears = async () => {
      setIsLoading(prev => ({ ...prev, years: true }));
      try {
        const response = await fetch('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/AcademicYear');
        if (!response.ok) throw new Error('Không thể tải danh sách năm học');
        const data = await response.json();
        setAcademicYears(data);
      } catch (err) {
        setError(err.message);
      } finally {
        setIsLoading(prev => ({ ...prev, years: false }));
      }
    };

    const fetchStudents = async () => {
      if (!selectedAcademicYear) return;
      setIsLoading(prev => ({ ...prev, students: true }));
      try {
        const response = await fetch(`https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Student/${selectedAcademicYear}`);
        if (!response.ok) throw new Error('Không thể tải danh sách học sinh');
        const data = await response.json();
        setStudentsData(data.students || []);
        setError(null);
      } catch (err) {
        setError(err.message);
      } finally {
        setIsLoading(prev => ({ ...prev, students: false }));
      }
    };

    fetchAcademicYears();
    fetchStudents();
  }, [selectedAcademicYear]);

  // Memoize unique classes
  const classes = useMemo(() =>
    [...new Set(studentsData.map(s => s.className))].filter(Boolean).sort(),
    [studentsData]
  );

  // Memoize filtered students
  const filteredStudents = useMemo(() => {
    return studentsData.filter(student => {
      const matchClass = !filters.className || student.className === filters.className;
      const matchGender = !filters.gender || student.gender === filters.gender;
      const matchSearch = !searchTerm ||
        (student.fullName && student.fullName.toLowerCase().includes(searchTerm.toLowerCase())) ||
        (student.studentId != null && String(student.studentId).toLowerCase().includes(searchTerm.toLowerCase()));
      return matchClass && matchGender && matchSearch;
    });
  }, [studentsData, filters, searchTerm]);

  // Pagination calculations
  const totalPages = Math.ceil(filteredStudents.length / itemsPerPage);
  const currentStudents = useMemo(() => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    return filteredStudents.slice(indexOfFirstItem, indexOfLastItem);
  }, [filteredStudents, currentPage, itemsPerPage]);

  // Generate pagination buttons with ellipsis
  const paginationButtons = useMemo(() => {
    const maxButtons = 5;
    const buttons = [];
    if (totalPages <= maxButtons) {
      return Array.from({ length: totalPages }, (_, i) => i + 1);
    }

    const start = Math.max(1, currentPage - 2);
    const end = Math.min(totalPages, currentPage + 2);

    if (start > 1) buttons.push(1);
    if (start > 2) buttons.push('...');
    for (let i = start; i <= end; i++) buttons.push(i);
    if (end < totalPages - 1) buttons.push('...');
    if (end < totalPages) buttons.push(totalPages);

    return buttons;
  }, [totalPages, currentPage]);

  // Handle student selection
  const handleSelectStudent = useCallback((studentId) => {
    setSelectedStudents(prev => {
      const newSelected = new Set(prev);
      newSelected.has(studentId) ? newSelected.delete(studentId) : newSelected.add(studentId);
      return newSelected;
    });
  }, []);

  // Handle select all
  const handleSelectAll = useCallback((e) => {
    setSelectedStudents(e.target.checked ? new Set(currentStudents.map(s => s.studentId)) : new Set());
  }, [currentStudents]);

  // Handle filter change
  const handleFilterChange = useCallback((field, value) => {
    setFilters(prev => ({ ...prev, [field]: value }));
    setSearchTerm(''); // Clear search term when filters change
    setCurrentPage(1);
    setSelectedStudents(new Set());
  }, []);

  // Handle search change
  const handleSearchChange = useCallback((value) => {
    setSearchTerm(value);
    setCurrentPage(1);
    setSelectedStudents(new Set());
  }, []);

  // Handle academic year change
  const handleAcademicYearChange = useCallback((value) => {
    setSelectedAcademicYear(value);
    setFilters({ className: '', gender: '' });
    setSearchTerm('');
    setCurrentPage(1);
    setSelectedStudents(new Set());
  }, []);

  // Handle page change
  const handlePageChange = useCallback((page) => {
    setCurrentPage(page);
    setSelectedStudents(new Set());
  }, []);

  // Handle items per page change
  const handleItemsPerPageChange = useCallback((value) => {
    setItemsPerPage(value);
    setCurrentPage(1);
    setSelectedStudents(new Set());
  }, []);

  // Handle send message
  const handleSendMessage = useCallback(() => {
    setShowConfirmDialog(true);
  }, []);

  // Confirm send message
  const confirmSendMessage = useCallback(async () => {
    setIsSending(true);
    setSendError(null);
    try {
      // Collect emails from selected students' parents
      const selectedStudentsList = currentStudents.filter(student =>
        selectedStudents.has(student.studentId)
      );

      // Create array of parent emails (father and mother)
      const parentEmails = selectedStudentsList.reduce((emails, student) => {
        if (student.parent?.emailFather) emails.push(student.parent.emailFather);
        if (student.parent?.emailMother) emails.push(student.parent.emailMother);
        return emails;
      }, []);

      // Check if there are any emails
      if (parentEmails.length === 0) {
        throw new Error('Không tìm thấy email phụ huynh nào để gửi thông báo');
      }

      const notificationData = {
        emails: parentEmails,
        subject,
        body: message,
        isHtml
      };

      const response = await fetch('https://hgsmapi-dsf3dzaxgpfyhua4.eastasia-01.azurewebsites.net/api/Notification/send-by-emails', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
          Authorization: `Bearer ${localStorage.getItem('token')?.replace(/^"|"$/g, '')}`
        },
        body: JSON.stringify(notificationData)
      });

      if (!response.ok) throw new Error('Không thể gửi thông báo');

      // Reset form after successful send
      setMessage('');
      setSubject('');
      setSelectedStudents(new Set());
      setShowConfirmDialog(false);
    } catch (err) {
      setSendError(err.message);
    } finally {
      setIsSending(false);
    }
  }, [message, subject, isHtml, selectedStudents, currentStudents]);

  // Error handling
  if (error) {
    return (
      <div className="p-4 text-red-500 text-center">
        <p>Lỗi: {error}</p>
        <button
          onClick={() => setError(null)}
          className="mt-2 px-4 py-2 bg-[#00598A] text-white rounded-lg hover:bg-[#6ca599]"
        >
          Thử lại
        </button>
      </div>
    );
  }

  return (
    <div className="p-4 max-w-7xl mx-auto">
      {/* Bộ lọc */}
      <div className="bg-white rounded-lg p-4 mb-4 shadow-sm">
        <div className="grid grid-cols-1 md:grid-cols-4 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="academicYear">
              Năm học
            </label>
            <select
              id="academicYear"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#00598A]"
              value={selectedAcademicYear}
              onChange={(e) => handleAcademicYearChange(e.target.value)}
              disabled={isLoading.years || !academicYears.length}
            >
              <option value="">Chọn năm học</option>
              {academicYears.map(year => (
                <option key={year.academicYearID} value={year.academicYearID}>
                  {year.yearName}
                </option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="className">
              Lớp
            </label>
            <select
              id="className"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#00598A]"
              value={filters.className}
              onChange={(e) => handleFilterChange('className', e.target.value)}
            >
              <option value="">Tất cả lớp</option>
              {classes.map(cls => (
                <option key={cls} value={cls}>{cls}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="gender">
              Giới tính
            </label>
            <select
              id="gender"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#00598A]"
              value={filters.gender}
              onChange={(e) => handleFilterChange('gender', e.target.value)}
            >
              <option value="">Tất cả</option>
              <option value="Nam">Nam</option>
              <option value="Nữ">Nữ</option>
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="search">
              Tìm kiếm học sinh
            </label>
            <input
              id="search"
              type="text"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#00598A]"
              value={searchTerm}
              onChange={(e) => handleSearchChange(e.target.value)}
              placeholder="Tìm theo tên hoặc mã học sinh..."
              aria-label="Tìm kiếm học sinh"
            />
          </div>
        </div>
      </div>

      {/* Bảng học sinh */}
      <div className="bg-white rounded-lg shadow-sm">
        <div className="flex items-center justify-between p-4 bg-gray-50">
          <h2 className="text-sm font-medium">Danh sách học sinh</h2>
          <span className="text-sm">Tổng cộng: {filteredStudents.length} học sinh</span>
        </div>

        <div className="overflow-x-auto">
          {isLoading.students ? (
            <div className="flex items-center justify-center p-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#00598A]"></div>
            </div>
          ) : (
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-[#00598A] text-white">
                  <th className="w-8 p-2">
                    <input
                      type="checkbox"
                      checked={selectedStudents.size === currentStudents.length && currentStudents.length > 0}
                      onChange={handleSelectAll}
                      aria-label="Chọn tất cả học sinh"
                    />
                  </th>
                  <th className="p-2 text-left">Họ và tên</th>
                  <th className="p-2 text-left">Mã học sinh</th>
                  <th className="p-2 text-left">Lớp</th>
                  <th className="p-2 text-left">Ngày sinh</th>
                  <th className="p-2 text-left">SĐT Bố</th>
                  <th className="p-2 text-left">SĐT Mẹ</th>
                  <th className="p-2 text-left">Email Bố</th>
                  <th className="p-2 text-left">Email Mẹ</th>
                </tr>
              </thead>
              <tbody>
                {currentStudents.map((student) => (
                  <tr key={student.studentId} className="border-b hover:bg-gray-50">
                    <td className="p-2">
                      <input
                        type="checkbox"
                        checked={selectedStudents.has(student.studentId)}
                        onChange={() => handleSelectStudent(student.studentId)}
                        aria-label={`Chọn ${student.fullName}`}
                      />
                    </td>
                    <td className="p-2">{student.fullName}</td>
                    <td className="p-2">{student.studentId}</td>
                    <td className="p-2">{student.className}</td>
                    <td className="p-2">{student.dob}</td>
                    <td className="p-2">{student.parent.phoneNumberFather || 'Chưa cập nhật'}</td>
                    <td className="p-2">{student.parent.phoneNumberMother || 'Chưa cập nhật'}</td>
                    <td className="p-2">{student.parent.emailFather || 'Chưa cập nhật'}</td>
                    <td className="p-2">{student.parent.emailMother || 'Chưa cập nhật'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Phân trang */}
        <div className="flex items-center justify-between p-4 border-t">
          <div className="flex items-center gap-2">
            <select
              className="border p-1 rounded text-sm focus:ring-2 focus:ring-[#00598A]"
              value={itemsPerPage}
              onChange={(e) => handleItemsPerPageChange(Number(e.target.value))}
              aria-label="Số mục mỗi trang"
            >
              <option value={10}>10</option>
              <option value={20}>20</option>
              <option value={50}>50</option>
            </select>
            <span className="text-sm">Đã chọn: {selectedStudents.size} mẫu tin</span>
          </div>
          <div className="flex gap-1">
            {paginationButtons.map((page, index) => (
              <button
                key={index}
                className={`px-3 py-1 border rounded hover:bg-gray-100 ${page === currentPage ? 'bg-[#00598A] text-white' : page === '...' ? 'cursor-default' : ''
                  }`}
                onClick={() => typeof page === 'number' && handlePageChange(page)}
                aria-label={typeof page === 'number' ? `Trang ${page}` : 'Chuyển trang'}
                disabled={page === '...'}
              >
                {page}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Form gửi thông báo */}
      <div className="bg-white rounded-lg p-4 mt-4 shadow-sm">
        <div className="text-sm font-medium mb-2">
          Người nhận: {selectedStudents.size} phụ huynh
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium mb-1" htmlFor="subject">
            Tiêu đề thông báo
          </label>
          <input
            id="subject"
            type="text"
            className="w-full border rounded-lg p-2 text-sm focus:ring-2 focus:ring-[#00598A]"
            value={subject}
            onChange={(e) => setSubject(e.target.value)}
            placeholder="Nhập tiêu đề thông báo..."
            aria-label="Tiêu đề thông báo"
          />
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium mb-1" htmlFor="message">
            Nội dung thông báo
          </label>
          <textarea
            id="message"
            className="w-full border rounded-lg p-2 min-h-[100px] resize-none focus:ring-2 focus:ring-[#00598A]"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            placeholder="Nhập nội dung thông báo..."
            aria-label="Nội dung thông báo"
          />
        </div>
        <div className="flex items-center gap-2 mb-2">
          <input
            type="checkbox"
            checked={isHtml}
            onChange={(e) => setIsHtml(e.target.checked)}
            id="isHtml"
            aria-label="Gửi dưới dạng HTML"
          />
          <label htmlFor="isHtml" className="text-sm">
            Định dạng HTML
          </label>
        </div>
        <div className="flex justify-end mt-2">
          <button
            onClick={handleSendMessage}
            className="px-4 py-1 bg-[#00598A] text-white rounded-lg text-sm hover:bg-[#6ca599] disabled:bg-gray-400"
            disabled={!message.trim() || !subject.trim() || selectedStudents.size === 0 || isSending}
            aria-label="Gửi thông báo"
          >
            {isSending ? 'Đang gửi...' : 'Gửi'}
          </button>
        </div>
        {sendError && (
          <p className="text-red-500 text-sm mt-2">
            Lỗi gửi thông báo: {sendError || 'Vui lòng thử lại.'}
          </p>
        )}
      </div>

      {/* Hộp thoại xác nhận */}
      {showConfirmDialog && (
        <div className="fixed inset-0 flex items-center justify-center backdrop-blur-sm z-50">
          <div className="bg-white p-4 rounded-lg max-w-md shadow-lg">
            <h3 className="text-lg font-medium mb-2">Xác nhận gửi thông báo</h3>
            <p className="text-sm mb-4">
              Bạn có chắc muốn gửi thông báo đến {selectedStudents.size} phụ huynh?
            </p>
            <div className="flex justify-end gap-2">
              <button
                onClick={() => setShowConfirmDialog(false)}
                className="px-4 py-1 border rounded text-sm hover:bg-gray-100"
                aria-label="Hủy"
              >
                Hủy
              </button>
              <button
                onClick={confirmSendMessage}
                className="px-4 py-1 bg-[#00598A] text-white rounded-lg text-sm hover:bg-[#6ca599]"
                aria-label="Xác nhận gửi"
              >
                Xác nhận
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
};

export default ContactParents;