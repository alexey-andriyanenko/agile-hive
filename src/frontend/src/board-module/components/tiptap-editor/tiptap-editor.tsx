import React, { useEffect, useState } from "react";
import { EditorContent, type JSONContent, useEditor } from "@tiptap/react";
import StarterKit from "@tiptap/starter-kit";
import Link from "@tiptap/extension-link";
import { Box, Button, Flex, Input, Dialog } from "@chakra-ui/react";
import { LuBold, LuItalic, LuList, LuLink } from "react-icons/lu";

import "./tiptap-editor.css";

type TipTapEditorProps = {
  value: JSONContent;
  onChange: (value: JSONContent) => void;
  className?: string;
};

export const TipTapEditor: React.FC<TipTapEditorProps> = ({ value, onChange, className }) => {
  const editor = useEditor({
    extensions: [
      StarterKit,
      Link.configure({
        openOnClick: false,
        autolink: false,
      }),
    ],
    content: value,
    onUpdate: ({ editor }) => {
      console.log("Editor content updated:", editor.getJSON());
      onChange(editor.getJSON());
    },
  });

  useEffect(() => {
    if (editor && JSON.stringify(value) !== JSON.stringify(editor.getJSON())) {
      editor.commands.setContent(value);
    }
  }, [value, editor]);

  const [isLinkDialogOpen, setIsLinkDialogOpen] = useState(false);
  const [linkValue, setLinkValue] = useState("");
  const [linkText, setLinkText] = useState("");

  // Robust delegated handler: intercept mousedown/click/auxclick in capture phase
  useEffect(() => {
    if (!editor) return;

    const view = editor.view;

    const handleLinkInteraction = (event: MouseEvent) => {
      const target = event.target as HTMLElement | null;
      if (!target) return;

      // Find the nearest <a> ancestor
      const anchor = target.closest("a") as HTMLElement | null;
      if (!anchor) return;

      // Make sure this anchor belongs to our editor (avoid intercepting other anchors on page)
      if (!view.dom.contains(anchor)) return;

      // Prevent native navigation (left click, middle click, ctrl/cmd+click, etc.)
      event.preventDefault();
      event.stopPropagation();

      // Read href and text from DOM anchor
      const href = anchor.getAttribute("href") || "";
      const text = anchor.textContent || "";

      // Try to compute ProseMirror doc range for the anchor by mapping DOM -> doc positions
      let fromPos: number | null = null;
      let toPos: number | null = null;

      try {
        // start at offset 0 inside anchor, end at number of child nodes
        fromPos = view.posAtDOM(anchor, 0);
        toPos = view.posAtDOM(anchor, anchor.childNodes.length);
        // eslint-disable-next-line @typescript-eslint/no-unused-vars
      } catch (err) {
        // ignore — we'll fall back below
      }

      if (typeof fromPos === "number" && typeof toPos === "number") {
        // Set selection to the entire link text so later commands operate on that range
        editor.chain().focus().setTextSelection({ from: fromPos, to: toPos }).run();
      } else {
        // fallback: try to get a position at the mouse coordinates
        const coords = { left: (event as MouseEvent).clientX, top: (event as MouseEvent).clientY };

        const posAt = view.posAtCoords(coords as { left: number; top: number } | never);
        if (posAt) {
          editor.chain().focus().setTextSelection(posAt.pos).run();
        }
      }

      // Prefill dialog inputs and open it
      setLinkValue(href);
      setLinkText(text);
      setIsLinkDialogOpen(true);
    };

    // Attach in capture phase to intercept before browser default navigation
    ["mousedown", "click", "auxclick"].forEach((evt) =>
      view.dom.addEventListener(evt, handleLinkInteraction as EventListener, true),
    );

    return () => {
      ["mousedown", "click", "auxclick"].forEach((evt) =>
        view.dom.removeEventListener(evt, handleLinkInteraction as EventListener, true),
      );
    };
  }, [editor]);

  const handleSetLink = () => {
    if (!editor || !linkValue) {
      setIsLinkDialogOpen(false);
      return;
    }

    const { empty, from, to } = editor.state.selection;

    // Use chain() consistently — avoids the "insertContent is not a function" error
    if (!empty) {
      // Replace the selected range with new text that has the link mark
      const replacementText = linkText || editor.state.doc.textBetween(from, to, " ");
      editor
        .chain()
        .focus()
        .deleteRange({ from, to })
        .insertContent({
          type: "text",
          text: replacementText,
          marks: [{ type: "link", attrs: { href: linkValue } }],
        })
        .run();
    } else {
      // Collapsed selection: insert new linked text at the cursor
      editor
        .chain()
        .focus()
        .insertContent({
          type: "text",
          text: linkText || linkValue,
          marks: [{ type: "link", attrs: { href: linkValue } }],
        })
        .run();
    }

    setLinkValue("");
    setLinkText("");
    setIsLinkDialogOpen(false);
  };

  const handleRemoveLink = () => {
    if (!editor) return;
    editor.chain().focus().unsetLink().run();
    setLinkValue("");
    setLinkText("");
    setIsLinkDialogOpen(false);
  };

  return (
    <Box width="100%" borderWidth="1px" borderColor="gray.800" borderRadius="md" p={4}>
      <Flex gap={2} mb={4}>
        <Button
          variant="outline"
          size="sm"
          colorScheme={editor.isActive("bold") ? "blue" : "gray"}
          color={editor.isActive("bold") ? "white" : "gray.100"}
          borderColor={editor.isActive("bold") ? "blue.500" : "gray.600"}
          onClick={() => editor?.chain().focus().toggleBold().run()}
          disabled={!editor}
        >
          <LuBold />
          Bold
        </Button>
        <Button
          variant="outline"
          size="sm"
          colorScheme={editor.isActive("italic") ? "blue" : "gray"}
          color={editor.isActive("italic") ? "white" : "gray.100"}
          borderColor={editor.isActive("italic") ? "blue.500" : "gray.600"}
          onClick={() => editor?.chain().focus().toggleItalic().run()}
          disabled={!editor}
        >
          <LuItalic />
          Italic
        </Button>
        <Button
          variant="outline"
          size="sm"
          colorScheme={editor.isActive("bulletList") ? "blue" : "gray"}
          color={editor.isActive("bulletList") ? "white" : "gray.100"}
          borderColor={editor.isActive("bulletList") ? "blue.500" : "gray.600"}
          onClick={() => editor?.chain().focus().toggleBulletList().run()}
          disabled={!editor}
        >
          <LuList />
          Bullet List
        </Button>
        <Button
          variant="outline"
          size="sm"
          colorScheme="gray"
          color="gray.100"
          borderColor="gray.600"
          onClick={() => {
            setLinkValue("");
            setLinkText("");
            setIsLinkDialogOpen(true);
          }}
          disabled={!editor}
        >
          <LuLink />
          Add Link
        </Button>
      </Flex>
      <EditorContent editor={editor} className={className} />

      <Dialog.Root lazyMount placement="center" open={isLinkDialogOpen}>
        <Dialog.Backdrop />
        <Dialog.Positioner>
          <Dialog.Content bg="gray.800" color="gray.100">
            <Dialog.Header justifyContent="center">
              <Dialog.Title>{editor?.isActive("link") ? "Edit Link" : "Insert Link"}</Dialog.Title>
            </Dialog.Header>
            <Dialog.Body display="flex" flexDirection="column" gap={2}>
              <Input
                placeholder="Enter URL"
                value={linkValue}
                onChange={(e) => setLinkValue(e.target.value)}
                autoFocus
                bg="gray.700"
                color="gray.100"
                borderColor="gray.600"
              />
              <Input
                placeholder="Enter Link Text"
                value={linkText}
                onChange={(e) => setLinkText(e.target.value)}
                bg="gray.700"
                color="gray.100"
                borderColor="gray.600"
              />
            </Dialog.Body>
            <Dialog.Footer>
              <Button size="sm" mr={2} onClick={handleSetLink} colorScheme="blue">
                {editor?.isActive("link") ? "Update" : "Insert"}
              </Button>
              {editor?.isActive("link") && (
                <Button size="sm" mr={2} onClick={handleRemoveLink} colorScheme="red">
                  Remove
                </Button>
              )}
              <Button size="sm" onClick={() => setIsLinkDialogOpen(false)} colorScheme="gray">
                Cancel
              </Button>
            </Dialog.Footer>
          </Dialog.Content>
        </Dialog.Positioner>
      </Dialog.Root>
    </Box>
  );
};
