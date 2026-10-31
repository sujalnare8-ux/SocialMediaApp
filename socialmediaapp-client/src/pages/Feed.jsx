import { useEffect, useState } from "react";
import { useNavigate, Link } from "react-router-dom";
import api from "../api/api";

function PostCard({ post, myId, onDelete }) {
  const [likeCount, setLikeCount] = useState(post.likeCount);
  const [liked, setLiked] = useState(post.likedByMe);
  const [commentCount, setCommentCount] = useState(post.commentCount);
  const [showComments, setShowComments] = useState(false);
  const [comments, setComments] = useState([]);
  const [text, setText] = useState("");

  const toggleLike = async () => {
    const res = await api.post(`/posts/${post.id}/like`);
    setLiked(res.data.liked);
    setLikeCount(res.data.likeCount);
  };

  const toggleComments = async () => {
    if (!showComments) {
      const res = await api.get(`/posts/${post.id}/comments`);
      setComments(res.data);
    }
    setShowComments(!showComments);
  };

  const addComment = async (e) => {
    e.preventDefault();
    if (!text.trim()) return;
    const res = await api.post(`/posts/${post.id}/comments`, { content: text });
    setComments([...comments, res.data]);
    setCommentCount(commentCount + 1);
    setText("");
  };

  return (
    <div style={{ border: "1px solid #888", borderRadius: 6, padding: 10, marginBottom: 10 }}>
      <Link to={`/profile/${post.userId}`}>
        <strong>{post.authorName}</strong>
      </Link>
      <small style={{ marginLeft: 8 }}>
        {new Date(post.createdAt + "Z").toLocaleString()}
      </small>
      <p>{post.content}</p>

      {post.imageUrl && (
        <img
          src={post.imageUrl}
          alt="post"
          style={{ maxWidth: "100%", borderRadius: 6, marginBottom: 8 }}
        />
      )}

      <button onClick={toggleLike}>
        {liked ? "❤️ Liked" : "🤍 Like"} ({likeCount})
      </button>{" "}
      <button onClick={toggleComments}>💬 Comments ({commentCount})</button>
      {post.userId === myId && (
        <button style={{ float: "right" }} onClick={() => onDelete(post.id)}>
          Delete
        </button>
      )}

      {showComments && (
        <div style={{ marginTop: 10 }}>
          {comments.length === 0 && <small>No comments yet.</small>}
          {comments.map((c) => (
            <p key={c.id} style={{ margin: "4px 0" }}>
              <strong>{c.authorName}:</strong> {c.content}
            </p>
          ))}
          <form onSubmit={addComment}>
            <input
              placeholder="Write a comment..."
              value={text}
              maxLength={500}
              onChange={(e) => setText(e.target.value)}
            />{" "}
            <button type="submit">Send</button>
          </form>
        </div>
      )}
    </div>
  );
}

export default function Feed() {
  const [posts, setPosts] = useState([]);
  const [content, setContent] = useState("");
  const [image, setImage] = useState(null);
  const [error, setError] = useState("");
  const [tab, setTab] = useState("following");
  const navigate = useNavigate();
  const name = localStorage.getItem("displayName");

  const loadPosts = async () => {
    try {
      setError("");
      const res = await api.get(tab === "following" ? "/posts/following" : "/posts");
      setPosts(res.data);
    } catch (err) {
      if (err.response?.status === 401) {
        localStorage.clear();
        navigate("/login");
      } else {
        setError("Could not load posts.");
      }
    }
  };

  useEffect(() => {
    loadPosts();
  }, [tab]);

  const createPost = async (e) => {
    e.preventDefault();
    if (!content.trim()) return;
    const formEl = e.target;
    try {
      setError("");
      let imageUrl = null;
      if (image) {
        const form = new FormData();
        form.append("file", image);
        const up = await api.post("/upload", form);
        imageUrl = up.data.url;
      }
      const res = await api.post("/posts", { content, imageUrl });
      setPosts([res.data, ...posts]);
      setContent("");
      setImage(null);
      formEl.reset();
    } catch (err) {
      const msg = err.response?.data;
      setError(typeof msg === "string" ? msg : "Could not create post.");
    }
  };

  const deletePost = async (id) => {
    try {
      await api.delete(`/posts/${id}`);
      setPosts(posts.filter((p) => p.id !== id));
    } catch (err) {
      setError("Could not delete post.");
    }
  };

  const logout = () => {
    localStorage.clear();
    navigate("/login");
  };

  const myId = JSON.parse(atob(localStorage.getItem("token").split(".")[1]))[
    "http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier"
  ];

  return (
    <div style={{ maxWidth: 600, margin: "40px auto", padding: 10 }}>
      <div style={{ display: "flex", justifyContent: "space-between" }}>
        <h2>Hello, {name}</h2>
        <div>
          <Link to={`/profile/${myId}`}>My profile</Link>{" "}
                    <Link to="/chat">Chat</Link>{" "}
          <button onClick={logout}>Logout</button>
        </div>
      </div>

      <form onSubmit={createPost}>
        <textarea
          rows={3}
          style={{ width: "100%" }}
          placeholder="What's on your mind?"
          value={content}
          maxLength={1000}
          onChange={(e) => setContent(e.target.value)}
        />
        <input
          type="file"
          accept="image/*"
          onChange={(e) => setImage(e.target.files[0])}
        />{" "}
        <button type="submit">Post</button>
      </form>

      {error && <p style={{ color: "red" }}>{error}</p>}

      <hr />

      <button onClick={() => setTab("following")} disabled={tab === "following"}>
        Following
      </button>{" "}
      <button onClick={() => setTab("all")} disabled={tab === "all"}>
        Everyone
      </button>
      <br /><br />

      {posts.length === 0 && <p>No posts yet. Write the first one!</p>}

      {posts.map((p) => (
        <PostCard key={p.id} post={p} myId={myId} onDelete={deletePost} />
      ))}
    </div>
  );
}