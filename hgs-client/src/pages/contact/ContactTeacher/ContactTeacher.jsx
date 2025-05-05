import React, { useState, useMemo, useCallback } from 'react';
import { useTeachers } from '../../../services/teacher/queries';
import { useSendNotification } from '../../../services/notification/mutations';

const ContactTeacher = () => {
  const [message, setMessage] = useState('');
  const [subject, setSubject] = useState('');
  const [isFirstMessage, setIsFirstMessage] = useState(true);
  const [isHtml, setIsHtml] = useState(false);
  const [selectedTeachers, setSelectedTeachers] = useState(new Set());
  const [filters, setFilters] = useState({
    department: '',
    gender: '',
  });
  const [currentPage, setCurrentPage] = useState(1);
  const [itemsPerPage, setItemsPerPage] = useState(10);
  const [showConfirmDialog, setShowConfirmDialog] = useState(false);

  const { data: teachersData, isLoading, error, refetch } = useTeachers();
  const { mutate: sendNotification, isLoading: isSending, error: sendError } = useSendNotification();

  // Handle nested teachers data from API
  const teachers = useMemo(() =>
    Array.isArray(teachersData?.teachers) ? teachersData.teachers : [],
    [teachersData]
  );

  // Memoize unique departments
  const departments = useMemo(() =>
    [...new Set(teachers.map(t => t.department))].filter(Boolean).sort(),
    [teachers]
  );

  // Memoize filtered teachers
  const filteredTeachers = useMemo(() => {
    return teachers.filter(teacher => {
      const matchDepartment = !filters.department || teacher.department === filters.department;
      const matchGender = !filters.gender || teacher.gender === filters.gender;
      return matchDepartment && matchGender;
    });
  }, [teachers, filters]);

  // Pagination calculations
  const totalPages = Math.ceil(filteredTeachers.length / itemsPerPage);
  const currentTeachers = useMemo(() => {
    const indexOfLastItem = currentPage * itemsPerPage;
    const indexOfFirstItem = indexOfLastItem - itemsPerPage;
    return filteredTeachers.slice(indexOfFirstItem, indexOfLastItem);
  }, [filteredTeachers, currentPage, itemsPerPage]);

  // Handle teacher selection
  const handleSelectTeacher = useCallback((teacherId) => {
    setSelectedTeachers(prev => {
      const newSelected = new Set(prev);
      if (newSelected.has(teacherId)) {
        newSelected.delete(teacherId);
      } else {
        newSelected.add(teacherId);
      }
      return newSelected;
    });
  }, []);

  // Handle select all
  const handleSelectAll = useCallback((e) => {
    if (e.target.checked) {
      const newSelected = new Set(currentTeachers.map(t => t.teacherId));
      setSelectedTeachers(newSelected);
    } else {
      setSelectedTeachers(new Set());
    }
  }, [currentTeachers]);

  const handleFilterChange = useCallback((field, value) => {
    setFilters(prev => ({
      ...prev,
      [field]: value,
    }));
    setCurrentPage(1);
    setSelectedTeachers(new Set()); // Reset selections when filters change
  }, []);

  const handlePageChange = useCallback((page) => {
    setCurrentPage(page);
    setSelectedTeachers(new Set()); // Reset selections when page changes
  }, []);

  const handleItemsPerPageChange = useCallback((value) => {
    setItemsPerPage(value);
    setCurrentPage(1);
    setSelectedTeachers(new Set()); // Reset selections when items per page changes
  }, []);

  const handleSendMessage = useCallback(() => {
    setShowConfirmDialog(true);
  }, []);

  const confirmSendMessage = useCallback(() => {
    const notificationData = {
      teacherIds: Array.from(selectedTeachers),
      subject,
      body: message,
      isHtml,
    };

    sendNotification(notificationData, {
      onSuccess: () => {
        setMessage('');
        setSubject('');
        setSelectedTeachers(new Set());
        setShowConfirmDialog(false);
      },
      onError: () => {
        // Error message is displayed below the form
      },
    });
  }, [sendNotification, selectedTeachers, subject, message, isHtml]);

  // Error handling for teacher data fetch
  if (error) {
    return (
      <div className="p-4 text-red-500 text-center">
        <p>Có lỗi xảy ra khi tải dữ liệu: {error.message}</p>
        {error.response?.status === 401 && (
          <p>Vui lòng đăng nhập lại.</p>
        )}
        <button
          onClick={refetch}
          className="mt-2 px-4 py-2 bg-[#7DB6AD] text-white rounded-lg hover:bg-[#6ca599]"
        >
          Thử lại
        </button>
      </div>
    );
  }

  return (
    <div className="p-4 max-w-7xl mx-auto">
      {/* Filters */}
      <div className="bg-white rounded-lg p-4 mb-4 shadow-sm">
        <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="department">
              Tổ bộ môn
            </label>
            <select
              id="department"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#7DB6AD]"
              value={filters.department}
              onChange={(e) => handleFilterChange('department', e.target.value)}
            >
              <option value="">Tất cả</option>
              {departments.map(dept => (
                <option key={dept} value={dept}>{dept}</option>
              ))}
            </select>
          </div>
          <div>
            <label className="block text-sm font-medium mb-1" htmlFor="gender">
              Giới tính
            </label>
            <select
              id="gender"
              className="w-full border rounded p-2 text-sm focus:ring-2 focus:ring-[#7DB6AD]"
              value={filters.gender}
              onChange={(e) => handleFilterChange('gender', e.target.value)}
            >
              <option value="">Tất cả</option>
              <option value="Nam">Nam</option>
              <option value="Nữ">Nữ</option>
            </select>
          </div>
        </div>
      </div>

      {/* Teachers Table */}
      <div className="bg-white rounded-lg shadow-sm">
        <div className="flex items-center justify-between p-4 bg-gray-50">
          <h2 className="text-sm font-medium">Danh sách giáo viên</h2>
          <span className="text-sm">Toàn trường: {filteredTeachers.length} Giáo viên</span>
        </div>

        <div className="overflow-x-auto">
          {isLoading ? (
            <div className="flex items-center justify-center p-8">
              <div className="animate-spin rounded-full h-8 w-8 border-b-2 border-[#7DB6AD]"></div>
            </div>
          ) : (
            <table className="w-full text-sm">
              <thead>
                <tr className="bg-[#7DB6AD] text-white">
                  <th className="w-8 p-2">
                    <input
                      type="checkbox"
                      checked={selectedTeachers.size === currentTeachers.length && currentTeachers.length > 0}
                      onChange={handleSelectAll}
                      aria-label="Select all teachers"
                    />
                  </th>
                  <th className="p-2 text-left">Họ và tên giáo viên</th>
                  <th className="p-2 text-left">Mã cán bộ</th>
                  <th className="p-2 text-left">Tổ bộ môn</th>
                  <th className="p-2 text-left">Số ĐTDĐ</th>
                  <th className="p-2 text-left">Email</th>
                </tr>
              </thead>
              <tbody>
                {currentTeachers.map((teacher) => (
                  <tr key={teacher.teacherId} className="border-b hover:bg-gray-50">
                    <td className="p-2">
                      <input
                        type="checkbox"
                        checked={selectedTeachers.has(teacher.teacherId)}
                        onChange={() => handleSelectTeacher(teacher.teacherId)}
                        aria-label={`Select ${teacher.fullName}`}
                      />
                    </td>
                    <td className="p-2">{teacher.fullName}</td>
                    <td className="p-2">{teacher.teacherId}</td>
                    <td className="p-2">{teacher.department}</td>
                    <td className="p-2">{teacher.phoneNumber || 'Chưa cập nhật'}</td>
                    <td className="p-2">{teacher.email || 'Chưa cập nhật'}</td>
                  </tr>
                ))}
              </tbody>
            </table>
          )}
        </div>

        {/* Pagination */}
        <div className="flex items-center justify-between p-4 border-t">
          <div className="flex items-center gap-2">
            <select
              className="border p-1 rounded text-sm focus:ring-2 focus:ring-[#7DB6AD]"
              value={itemsPerPage}
              onChange={(e) => handleItemsPerPageChange(Number(e.target.value))}
              aria-label="Items per page"
            >
              <option value={10}>10</option>
              <option value={20}>20</option>
              <option value={50}>50</option>
            </select>
            <span className="text-sm">Tổng số: {selectedTeachers.size} mẫu tin</span>
          </div>
          <div className="flex gap-1">
            {Array.from({ length: totalPages }, (_, i) => i + 1).map((page) => (
              <button
                key={page}
                className={`px-3 py-1 border rounded hover:bg-gray-100 ${currentPage === page ? 'bg-[#7DB6AD] text-white' : ''
                  }`}
                onClick={() => handlePageChange(page)}
                aria-label={`Page ${page}`}
              >
                {page}
              </button>
            ))}
          </div>
        </div>
      </div>

      {/* Message Form */}
      <div className="bg-white rounded-lg p-4 mt-4 shadow-sm">
        <div className="text-sm font-medium mb-2">
          Người nhận: {selectedTeachers.size} giáo viên
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium mb-1" htmlFor="subject">
            Tiêu đề thông báo
          </label>
          <input
            id="subject"
            type="text"
            className="w-full border rounded-lg p-2 text-sm focus:ring-2 focus:ring-[#7DB6AD]"
            value={subject}
            onChange={(e) => setSubject(e.target.value)}
            placeholder="Nhập tiêu đề thông báo..."
            aria-label="Notification subject"
          />
        </div>
        <div className="mb-2">
          <label className="block text-sm font-medium mb-1" htmlFor="message">
            Nội dung thông báo
          </label>
          <textarea
            id="message"
            className="w-full border rounded-lg p-2 min-h-[100px] resize-none focus:ring-2 focus:ring-[#7DB6AD]"
            value={message}
            onChange={(e) => setMessage(e.target.value)}
            placeholder="Nhập nội dung thông báo..."
            aria-label="Notification content"
          />
        </div>
        <div className="flex items-center gap-2 mb-2">
          <input
            type="checkbox"
            checked={isHtml}
            onChange={(e) => setIsHtml(e.target.checked)}
            id="isHtml"
            aria-label="Send as HTML"
          />
          <label htmlFor="isHtml" className="text-sm">
            Định dạng HTML
          </label>
        </div>
        <div className="flex items-center gap-2">
          <input
            type="checkbox"
            checked={isFirstMessage}
            onChange={(e) => setIsFirstMessage(e.target.checked)}
            id="firstMessage"
            aria-label="Send as first message"
          />
          <label htmlFor="firstMessage" className="text-sm">
            Gửi tin nhắn đầu tiên
          </label>
        </div>
        <div className="flex justify-end mt-2">
          <button
            onClick={handleSendMessage}
            className="px-4 py-1 bg-[#7DB6AD] text-white rounded-lg text-sm hover:bg-[#6ca599] disabled:bg-gray-400"
            disabled={!message.trim() || !subject.trim() || selectedTeachers.size === 0 || isSending}
            aria-label="Send notification"
          >
            {isSending ? 'Đang gửi...' : 'Gửi'}
          </button>
        </div>
        {sendError && (
          <p className="text-red-500 text-sm mt-2">
            Lỗi khi gửi thông báo: {sendError.message || 'Vui lòng thử lại.'}
          </p>
        )}
      </div>

      {/* Confirmation Dialog */}
      {showConfirmDialog && (
        <div className="fixed inset-0 flex items-center justify-center backdrop-blur-sm z-50">
          <div className="bg-white p-4 rounded-lg max-w-md shadow-lg">
            <h3 className="text-lg font-medium mb-2">Xác nhận gửi thông báo</h3>
            <p className="text-sm mb-4">
              Bạn có chắc muốn gửi thông báo đến {selectedTeachers.size} giáo viên?
            </p>
            <div className="flex justify-end gap-2">
              <button
                onClick={() => setShowConfirmDialog(false)}
                className="px-4 py-1 border rounded text-sm hover:bg-gray-100"
                aria-label="Cancel"
              >
                Hủy
              </button>
              <button
                onClick={confirmSendMessage}
                className="px-4 py-1 bg-[#7DB6AD] text-white rounded-lg text-sm hover:bg-[#6ca599]"
                aria-label="Confirm send"
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

export default ContactTeacher;