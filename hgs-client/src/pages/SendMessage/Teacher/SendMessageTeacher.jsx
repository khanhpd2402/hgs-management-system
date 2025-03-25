import React, { useState } from 'react';
import './SendMessageTeacher.scss';

const SendMessageTeacher = () => {
    const [activeSection, setActiveSection] = useState('gvcn'); // Mặc định hiển thị GVCN

    // Dữ liệu cho từng mục
    const data = {
        overview: [
            { id: 1, name: 'Nguyễn Văn A', code: 'TQ-001', department: 'Ban Giám Hiệu', phone: '0123456789', messagesViettel: 2, messagesOther: 1 },
            { id: 2, name: 'Trần Thị B', code: 'TQ-002', department: 'Văn Phòng', phone: '0987654321', messagesViettel: 1, messagesOther: 0 },
        ],
        subjects: [
            { id: 1, name: 'Lê Văn C', code: 'GV-001', department: 'Toán', phone: '0123456789', messagesViettel: 0, messagesOther: 0 },
            { id: 2, name: 'Phạm Thị D', code: 'GV-002', department: 'Lý', phone: '0987654321', messagesViettel: 1, messagesOther: 1 },
        ],
        yearOfficers: [
            { id: 1, name: 'Hoàng Văn E', code: 'CB-001', department: 'Khối 10', phone: '0123456789', messagesViettel: 3, messagesOther: 0 },
            { id: 2, name: 'Ngô Thị F', code: 'CB-002', department: 'Khối 11', phone: '0987654321', messagesViettel: 2, messagesOther: 1 },
        ],
        classOfficers: [
            { id: 1, name: 'Đỗ Văn G', code: 'LT-001', department: '10A1', phone: '0123456789', messagesViettel: 1, messagesOther: 0 },
            { id: 2, name: 'Lý Thị H', code: 'LT-002', department: '11A2', phone: '0987654321', messagesViettel: 0, messagesOther: 2 },
        ],
        viettelPhones: [
            { id: 1, name: 'Trương Văn I', code: 'VT-001', department: 'Khối THPT', phone: '0123456789', messagesViettel: 5, messagesOther: 0 },
            { id: 2, name: 'Mai Thị K', code: 'VT-002', department: 'Khối THCS', phone: '0987654321', messagesViettel: 3, messagesOther: 0 },
        ],
        weeklyNews: [
            { id: 1, name: 'Bùi Văn L', code: 'TT-001', department: 'Tổ Văn Phòng', phone: '0123456789', messagesViettel: 2, messagesOther: 1 },
            { id: 2, name: 'Hồ Thị M', code: 'TT-002', department: 'Tổ Truyền Thông', phone: '0987654321', messagesViettel: 1, messagesOther: 1 },
        ],
        partyCell: [
            { id: 1, name: 'Đinh Văn N', code: 'CB-001', department: 'Chi bộ 1', phone: '0123456789', messagesViettel: 4, messagesOther: 0 },
            { id: 2, name: 'Chu Thị P', code: 'CB-002', department: 'Chi bộ 2', phone: '0987654321', messagesViettel: 2, messagesOther: 1 },
        ],
        schoolUnion: [
            { id: 1, name: 'Tống Văn Q', code: 'CD-001', department: 'BCH Công Đoàn', phone: '0123456789', messagesViettel: 3, messagesOther: 0 },
            { id: 2, name: 'Lưu Thị R', code: 'CD-002', department: 'Tổ Công Đoàn', phone: '0987654321', messagesViettel: 1, messagesOther: 2 },
        ],
        youthUnion: [
            { id: 1, name: 'Dương Văn S', code: 'TN-001', department: 'BCH Đoàn', phone: '0123456789', messagesViettel: 2, messagesOther: 1 },
            { id: 2, name: 'Kiều Thị T', code: 'TN-002', department: 'Chi Đoàn', phone: '0987654321', messagesViettel: 1, messagesOther: 0 },
        ],
        gvcn: [
            { id: 1, name: 'Tạ Thuấn Anh', code: '36366512-07-1-1', department: 'Khối học xã hội', phone: '0985836971', messagesViettel: 0, messagesOther: 0 },
            { id: 2, name: 'Vương Thị Ngọc Anh', code: '36366512-07-1-22', department: 'Khối học xã hội', phone: '0373450650', messagesViettel: 0, messagesOther: 0 },
            { id: 3, name: 'Nguyễn Thị Chiếm', code: '36366512-07-1-2', department: 'Tổ trưởng', phone: '094412500', messagesViettel: 0, messagesOther: 0 },
            { id: 4, name: 'Phạm Duy Duyên', code: '36366512-07-1-14', department: 'Khối học tự nhiên', phone: '083462358', messagesViettel: 0, messagesOther: 0 },
            { id: 5, name: 'Nguyễn Thị Duyên', code: '36366512-07-1-6', department: 'Tổ trưởng', phone: '097740375', messagesViettel: 0, messagesOther: 0 },
            { id: 6, name: 'Phạm Thị Hải', code: '36366512-07-1-4', department: 'Khối học xã hội', phone: '0857124936', messagesViettel: 0, messagesOther: 0 },
        ],
    };

    // Tiêu đề cho từng mục
    const sectionTitles = {
        overview: 'Tổng quan',
        subjects: 'Lớp học bộ môn',
        yearOfficers: 'Cán bộ năm',
        classOfficers: 'Cán bộ lớp',
        viettelPhones: 'Danh sách SĐT VIETTEL',
        weeklyNews: 'Bản tin hằng tuần',
        partyCell: 'Chi bộ',
        schoolUnion: 'Công đoàn trường',
        youthUnion: 'Đoàn thanh niên',
        gvcn: 'Giáo viên chủ nhiệm'
    };

    // Lấy data hiện tại dựa trên section đang active
    const currentData = data[activeSection] || [];

    return (
        <div className="container">
            {/* Sidebar */}
            <div className="sidebar">
                <h3 className="sidebar-title">Danh sách thống kê</h3>
                <ul className="sidebar-list">
                    {Object.entries(sectionTitles).map(([key, title]) => (
                        <li
                            key={key}
                            className={`sidebar-item ${activeSection === key ? 'active' : ''}`}
                            onClick={() => setActiveSection(key)}
                        >
                            {title}
                        </li>
                    ))}
                </ul>
            </div>

            {/* Main Content */}
            <div className="main-content">
                {/* Header */}
                <div className="header">
                    <h2 className="header-title">{sectionTitles[activeSection]}: {currentData.length} Giáo viên</h2>
                    <h3 className="sub-header">Nguời nhận: 0 giáo viên</h3>
                </div>

                {/* Teacher List */}
                <div className="table-container">
                    <h3 className="table-title">Danh sách {sectionTitles[activeSection].toLowerCase()}</h3>
                    <table className="table">
                        <thead>
                            <tr>
                                <th className="checkbox-column">
                                    <input type="checkbox" />
                                </th>
                                <th>Họ và tên</th>
                                <th>Mã cán bộ</th>
                                <th>Tổ/Bộ phận</th>
                                <th>Số ĐT ĐD</th>
                                <th>Số tin nhắn đã nhận Mạng Viettel</th>
                                <th>Số tin nhắn đã nhận Mạng khác</th>
                            </tr>
                        </thead>
                        <tbody>
                            {currentData.map((item) => (
                                <tr key={item.id}>
                                    <td className="checkbox-column">
                                        <input type="checkbox" />
                                    </td>
                                    <td>{item.name}</td>
                                    <td>{item.code}</td>
                                    <td>{item.department}</td>
                                    <td>{item.phone}</td>
                                    <td>{item.messagesViettel}</td>
                                    <td>{item.messagesOther}</td>
                                </tr>
                            ))}
                        </tbody>
                    </table>

                    {/* Pagination */}
                    <div className="pagination">
                        <span className="pagination-text">Tổng số: {currentData.length} - Hiển thị 1 - 10</span>
                        <div className="pagination-buttons">
                            <button>◄</button>
                            <button className="active">1</button>
                            <button>2</button>
                            <button>3</button>
                            <button>...</button>
                            <button>►</button>
                        </div>
                    </div>
                </div>

                {/* Message Section */}
                <div className="message-section">
                    <h3 className="message-title">Nguời nhận: 0 giáo viên</h3>
                    <input type="text" placeholder="Toàn trường" />
                    <textarea rows="4" placeholder=""></textarea>
                    <div className="message-footer">
                        <label className="checkbox-label">
                            <input type="checkbox" /> Gửi tin nhắn có dấu
                        </label>
                        <span className="char-count">0/0</span>
                        <button className="send-button">
                            Gửi <span className="send-icon">➤</span>
                        </button>
                    </div>
                </div>
            </div>
        </div>
    );
};

export default SendMessageTeacher;