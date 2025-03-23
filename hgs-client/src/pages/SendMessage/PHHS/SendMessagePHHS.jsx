// SendMessagePHHS.jsx
import React from 'react';
import './SendMessagePHHS.scss';
import classData from './classData';

const SendMessagePHHS = () => {
    return (
        <div className="container">
            <div className="header">
                <h2 className="headerTitle">Lớp học tính: 0 (0 PHHS)</h2>
                <button className="headerButton">Tất cả</button>
            </div>

            <table className="table">
                <thead>
                    <tr>
                        <th className="th"><input type="checkbox" /></th>
                        <th className="th">TT</th>
                        <th className="th">Lớp</th>
                        <th className="th">Giáo viên chủ nhiệm</th>
                        <th className="th">Số PHHS đã ký</th>
                        <th className="th">Tổng số</th>
                        <th className="th">Quy trình nhận (SMS/ĐÃ)</th>
                        <th className="th">Còn lại</th>
                    </tr>
                </thead>
                <tbody>
                    {classData.map((item) => (
                        <tr key={item.id} className="tr">
                            <td className="td"><input type="checkbox" /></td>
                            <td className="td">{item.id}</td>
                            <td className="td">{item.className}</td>
                            <td className="td">{item.teacher}</td>
                            <td className="td">{item.signed}</td>
                            <td className="td">{item.total}</td>
                            <td className="td">{item.process}</td>
                            <td className="td">{item.remaining}</td>
                        </tr>
                    ))}
                </tbody>
            </table>

            <div className="footer">
                <label className="footerLabel">Đúng tính THCS Hùng Giang:</label>
                <input type="text" className="footerInput" placeholder="" />
            </div>

            <div className="messageSection">
                <h3 className="messageTitle">Nội dung tin nhắn</h3>
                <label className="messageLabel">Trường THCS Hải Giang:</label>
                <input type="text" className="messageInput" placeholder="" />
                <div className="noteContainer">
                    <p className="noteText">Lưu ý: Nội dung tin nhắn có chứa [Tên học sinh]/[Tên lớp] nên để lại tin nhắn ở mỗi học sinh sẽ khác nhau.</p>
                    <span className="noteStats">231/1</span>
                </div>
            </div>
        </div>
    );
};

export default SendMessagePHHS;
